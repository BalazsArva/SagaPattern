using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Utilities.Extensions;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class AddStocksCommandHandler : IAddStocksCommandHandler
    {
        private readonly IDocumentStore _documentStore;

        public AddStocksCommandHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task HandleAsync(AddStocksCommand command, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var productCommandLookup = command.Stocks.ToDictionary(s => s.ProductId);
                var productLookup = await session.LoadProductsAsync(productCommandLookup.Keys, cancellationToken).ConfigureAwait(false);

                foreach (var pair in productLookup)
                {
                    var loadedProduct = pair.Value;

                    var changeVector = session.Advanced.GetChangeVectorFor(loadedProduct);
                    var productStockChange = productCommandLookup[pair.Key];

                    loadedProduct.StockCount += productStockChange.Quantity;

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                // TODO: Catch concurrency exception
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}