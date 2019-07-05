using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.DeliveryAPI.Entities;

namespace SagaDemo.DeliveryAPI.Extensions
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static Task<Delivery> LoadDeliveryAsync(this IAsyncDocumentSession session, string id, CancellationToken cancellationToken)
        {
            var internalId = DocumentIdHelper.GetDocumentId<Delivery>(session.Advanced.DocumentStore, id);

            return session.LoadAsync<Delivery>(internalId, cancellationToken);
        }
    }
}