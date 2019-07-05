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
        private static readonly PropertyInfo AttemptsPropertyInfo = typeof(StepDetails).GetProperty(nameof(StepDetails.Attempts));
        private static readonly PropertyInfo StepStatusPropertyInfo = typeof(StepDetails).GetProperty(nameof(StepDetails.StepStatus));

        public IDocumentStore DocumentStore { get; }

        protected TransactionOrchestratorBase(IDocumentStore documentStore)
        {
            DocumentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        protected async Task<TTransaction> GetTransactionByIdAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                return transactionDocument;
            }
        }

        protected async Task PerformTransactionStepAsync(
            string transactionId,
            Expression<Func<TTransaction, StepDetails>> stepSelector,
            Func<CancellationToken, Task<StepResult>> stepInvoker,
            CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var stepDetails = stepSelector.Compile()(transactionDocument);

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

                StepResult stepResult;
                var stepStatus = StepStatus.InProgress;
                var attempts = stepDetails.Attempts;

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
                    ++attempts;
                    stepStatus = StepStatus.TemporalFailure;

                    if (stepDetails.Attempts > MaxAttemptsPerStep)
                    {
                        stepStatus = StepStatus.PermanentFailure;
                    }
                }
                else if (stepResult == StepResult.Abort)
                {
                    stepStatus = StepStatus.PermanentFailure;
                }
                else
                {
                    stepStatus = StepStatus.Completed;
                }

                var attemptsMemberExpression = Expression.MakeMemberAccess(stepSelector, RollbackAttemptsPropertyInfo);
                var stepStatusMemberExpression = Expression.MakeMemberAccess(stepSelector, StepStatusPropertyInfo);

                var attemptsMemberAccessor = Expression.Lambda<Func<TTransaction, int>>(attemptsMemberExpression, stepSelector.Parameters);
                var stepStatusMemberAccessor = Expression.Lambda<Func<TTransaction, StepStatus>>(stepStatusMemberExpression, stepSelector.Parameters);

                session.Advanced.Patch(transactionDocument, attemptsMemberAccessor, attempts);
                session.Advanced.Patch(transactionDocument, stepStatusMemberAccessor, stepStatus);

                if (stepStatus == StepStatus.PermanentFailure)
                {
                    session.Advanced.Patch(transactionDocument, t => t.TransactionStatus, TransactionStatus.PermanentFailure);
                }

                if (stepStatus == StepStatus.TemporalFailure)
                {
                    session.Advanced.Patch(transactionDocument, t => t.UtcDoNotExecuteBefore, DateTime.UtcNow.AddSeconds(DefaultRetryDelaySeconds));
                }

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

                if (stepStatus != StepStatus.RolledBack)
                {
                    session.Advanced.Patch(transactionDocument, t => t.UtcDoNotExecuteBefore, DateTime.UtcNow.AddSeconds(DefaultRetryDelaySeconds));
                }

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