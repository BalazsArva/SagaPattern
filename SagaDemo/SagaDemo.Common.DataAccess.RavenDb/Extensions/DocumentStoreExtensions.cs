using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;

namespace SagaDemo.Common.DataAccess.RavenDb.Extensions
{
    public static class DocumentStoreExtensions
    {
        public static async Task ExecuteInConcurrentSessionAsync(this IDocumentStore documentStore, Func<IAsyncDocumentSession, CancellationToken, Task> operationExecutor, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    using (var session = documentStore.OpenAsyncSession())
                    {
                        await operationExecutor(session, cancellationToken).ConfigureAwait(false);

                        return;
                    }
                }
                catch (ConcurrencyException)
                {
                    // Ignore exception and retry
                }
            }
        }
    }
}