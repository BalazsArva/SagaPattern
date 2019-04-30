using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class TakeoutItemsCommandHandler : ITakeoutItemsCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<TakeoutItemsCommand> requestValidator;

        public TakeoutItemsCommandHandler(IDocumentStore documentStore, IValidator<TakeoutItemsCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                var productQuantityLookup = command.Items.ToDictionary(cmd => cmd.ProductId, cmd => cmd.Quantity);
                var productLookup = await session.LoadProductsAsync(productQuantityLookup.Keys, cancellationToken).ConfigureAwait(false);

                foreach (var pair in productLookup)
                {
                    var loadedProduct = pair.Value;
                    var changeVector = session.Advanced.GetChangeVectorFor(loadedProduct);

                    var quantity = productQuantityLookup[pair.Key];

                    loadedProduct.StockCount -= quantity;
                    loadedProduct.ReservationCount -= quantity;

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                // TODO: Catch concurrency exception
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}