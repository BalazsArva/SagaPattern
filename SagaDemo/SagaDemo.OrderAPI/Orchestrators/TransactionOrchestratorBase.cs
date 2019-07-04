using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.OrderAPI.Entitites;

namespace SagaDemo.OrderAPI.Orchestrators
{
    public abstract class TransactionOrchestratorBase<TTransaction> where TTransaction : TransactionBase
    {
        public const int DefaultRetryDelaySeconds = 15;
        public const int MaxAttemptsPerStep = 10;
        public const int MaxRollbackAttemptsPerStep = 10;

        private static readonly PropertyInfo RollbackAttemptsPropertyInfo = typeof(StepDetails).GetProperty(nameof(StepDetails.RollbackAttempts));
        private static readonly PropertyInfo StepStatusPropertyInfo = typeof(StepDetails).GetProperty(nameof(StepDetails.StepStatus));

        public IDocumentStore DocumentStore { get; }

        public TransactionOrchestratorBase(IDocumentStore documentStore)
        {
            DocumentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        protected async Task<(TTransaction transaction, string changeVector)> GetTransactionByIdAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                return (transactionDocument, changeVector);
            }
        }

        protected async Task PerformTransactionStepAsync(
            string transactionId,
            Func<TTransaction, StepDetails> stepSelector,
            Func<CancellationToken, Task<StepResult>> stepInvoker,
            CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);
                var stepDetails = stepSelector(transactionDocument);

                if (stepDetails.StepStatus == StepStatus.Completed ||
                    stepDetails.StepStatus == StepStatus.PermanentFailure ||
                    stepDetails.StepStatus == StepStatus.RollbackFailed ||
                    stepDetails.StepStatus == StepStatus.RolledBack)
                {
                    return;
                }

                if (transactionDocument.TransactionStatus != TransactionStatus.NotStarted ||
                    transactionDocument.TransactionStatus != TransactionStatus.InProgress)
                {
                    return;
                }

                stepDetails.StepStatus = StepStatus.InProgress;
                StepResult stepResult;

                try
                {
                    stepResult = await stepInvoker(cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    stepResult = StepResult.Retry;
                }

                if (stepResult == StepResult.Retry)
                {
                    stepDetails.StepStatus = StepStatus.TemporalFailure;
                    transactionDocument.UtcDoNotExecuteBefore = DateTime.UtcNow.AddSeconds(DefaultRetryDelaySeconds);
                    ++stepDetails.Attempts;

                    if (stepDetails.Attempts > MaxAttemptsPerStep)
                    {
                        stepDetails.StepStatus = StepStatus.PermanentFailure;
                    }
                }
                else if (stepResult == StepResult.Abort)
                {
                    stepDetails.StepStatus = StepStatus.PermanentFailure;
                }
                else
                {
                    stepDetails.StepStatus = StepStatus.Completed;
                }

                if (stepDetails.StepStatus == StepStatus.PermanentFailure)
                {
                    transactionDocument.TransactionStatus = TransactionStatus.PermanentFailure;
                }

                // TODO: Handle concurrent updates
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        protected async Task PerformRollbackStepAsync(
            string transactionId,
            Expression<Func<TTransaction, StepDetails>> stepSelector,
            Func<CancellationToken, Task> rollbackInvoker,
            CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var stepDetails = stepSelector.Compile()(transactionDocument);
                var stepStatus = stepDetails.StepStatus;
                var rollbackAttempts = stepDetails.RollbackAttempts;
                var transactionStatus = transactionDocument.TransactionStatus;

                if (stepStatus == StepStatus.RolledBack || stepStatus == StepStatus.NotStarted)
                {
                    return;
                }

                try
                {
                    await rollbackInvoker(cancellationToken).ConfigureAwait(false);

                    stepStatus = StepStatus.RolledBack;
                }
                catch
                {
                    ++rollbackAttempts;

                    if (rollbackAttempts > MaxRollbackAttemptsPerStep)
                    {
                        stepStatus = StepStatus.RollbackFailed;
                        transactionStatus = TransactionStatus.RollbackFailed;
                    }
                }

                var rollbackAttemptsMemberExpression = Expression.MakeMemberAccess(stepSelector, RollbackAttemptsPropertyInfo);
                var stepStatusMemberExpression = Expression.MakeMemberAccess(stepSelector, StepStatusPropertyInfo);

                var rollbackAttemptsMemberAccessor = Expression.Lambda<Func<TTransaction, int>>(rollbackAttemptsMemberExpression, stepSelector.Parameters);
                var stepStatusMemberAccessor = Expression.Lambda<Func<TTransaction, StepStatus>>(stepStatusMemberExpression, stepSelector.Parameters);

                session.Advanced.Patch(transactionDocument, t => t.TransactionStatus, transactionStatus);
                session.Advanced.Patch(transactionDocument, rollbackAttemptsMemberAccessor, rollbackAttempts);
                session.Advanced.Patch(transactionDocument, stepStatusMemberAccessor, stepStatus);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        protected async Task CompleteTransactionAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                session.Advanced.Patch(transactionDocument, t => t.TransactionStatus, TransactionStatus.Completed);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}