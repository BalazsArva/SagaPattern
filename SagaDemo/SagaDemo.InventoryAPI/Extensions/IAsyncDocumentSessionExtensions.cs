using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.InventoryAPI.Entities;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static Task<Product> LoadProductAsync(this IAsyncDocumentSession session, string id, CancellationToken cancellationToken)
        {
            var internalId = DocumentIdHelper.GetDocumentId<Product>(session, id);

            return session.LoadAsync<Product>(internalId, cancellationToken);
        }

        public static async Task<Dictionary<string, Product>> LoadProductsAsync(this IAsyncDocumentSession session, IEnumerable<string> ids, CancellationToken cancellationToken)
        {
            var internalIds = DocumentIdHelper.GetDocumentIds<Product>(session, ids);

            var products = await session.LoadAsync<Product>(internalIds, cancellationToken).ConfigureAwait(false);

            return products.ToDictionary(
                pair => DocumentIdHelper.GetEntityId<Product>(session, pair.Key),
                pair => pair.Value);
        }
    }
}