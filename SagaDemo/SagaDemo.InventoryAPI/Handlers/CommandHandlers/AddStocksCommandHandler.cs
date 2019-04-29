using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Utilities.Extensions;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class AddStocksCommandHandler : IAddStocksCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<AddStocksCommand> requestValidator;

        public AddStocksCommandHandler(IDocumentStore documentStore, IValidator<AddStocksCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddStocksCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
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