using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Exceptions;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Validation.Validators;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class TakeoutItemsCommandHandler : ITakeoutItemsCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IInventoryBatchCommandValidator<TakeoutItemsCommand> requestValidator;

        public TakeoutItemsCommandHandler(IDocumentStore documentStore, IInventoryBatchCommandValidator<TakeoutItemsCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await HandleInternalAsync(command, cancellationToken).ConfigureAwait(false);

                    return;
                }
                catch (ConcurrencyException)
                {
                    // Ignore, retry until either successfully updated or validation detects that there are not enough reservations or stocks.
                }
            }
        }

        private async Task HandleInternalAsync(TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var productQuantityLookup = command.Items.ToDictionary(cmd => cmd.ProductId, cmd => cmd.Quantity);
                var productLookup = await session.LoadProductsAsync(productQuantityLookup.Keys, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                foreach (var pair in productLookup)
                {
                    var loadedProduct = pair.Value;
                    var changeVector = session.Advanced.GetChangeVectorFor(loadedProduct);

                    var quantity = productQuantityLookup[pair.Key];

                    loadedProduct.StockCount -= quantity;
                    loadedProduct.ReservationCount -= quantity;

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}