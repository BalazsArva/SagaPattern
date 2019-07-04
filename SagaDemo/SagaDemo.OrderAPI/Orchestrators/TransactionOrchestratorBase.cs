using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Extensions;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.OrderAPI.Entitites;

namespace SagaDemo.OrderAPI.Orchestrators
{
    public abstract class TransactionOrchestratorBase<TTransaction> where TTransaction : TransactionBase
    {
        public const int DefaultRetryDelaySeconds = 15;
        public const int MaxAttemptsPerStep = 10;
        public const int MaxRollbackAttemptsPerStep = 10;

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
            Func<TTransaction, StepDetails> stepSelector,
            Func<CancellationToken, Task> rollbackInvoker,
            CancellationToken cancellationToken)
        {
            await DocumentStore
                .ExecuteInConcurrentSessionAsync(async (session, ct) =>
                {
                    var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                    var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, ct).ConfigureAwait(false);

                    var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);
                    var stepDetails = stepSelector(transactionDocument);
                    var stepStatus = stepDetails.StepStatus;

                    if (stepStatus == StepStatus.RolledBack || stepStatus == StepStatus.NotStarted)
                    {
                        return;
                    }

                    try
                    {
                        await rollbackInvoker(ct).ConfigureAwait(false);

                        stepDetails.StepStatus = StepStatus.RolledBack;
                    }
                    catch
                    {
                        ++stepDetails.RollbackAttempts;

                        if (stepDetails.RollbackAttempts > MaxRollbackAttemptsPerStep)
                        {
                            stepDetails.StepStatus = StepStatus.RollbackFailed;
                            transactionDocument.TransactionStatus = TransactionStatus.RollbackFailed;
                        }

                        // TODO: Retry with delay
                    }

                    await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, ct).ConfigureAwait(false);
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        protected async Task FinalizeTransactionAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = DocumentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<TTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<TTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.TransactionStatus = TransactionStatus.Completed;

                // TODO: Handle concurrency issues
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}