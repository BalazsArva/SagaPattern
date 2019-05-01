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
    public class BringbackItemsCommandHandler : IBringbackItemsCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IInventoryBatchCommandValidator<BringbackItemsCommand> requestValidator;

        public BringbackItemsCommandHandler(IDocumentStore documentStore, IInventoryBatchCommandValidator<BringbackItemsCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(BringbackItemsCommand command, CancellationToken cancellationToken)
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
                    // Ignore, retry until successfully updated
                }
            }
        }

        private async Task HandleInternalAsync(BringbackItemsCommand command, CancellationToken cancellationToken)
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

                    loadedProduct.StockCount += productQuantityLookup[pair.Key];

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}