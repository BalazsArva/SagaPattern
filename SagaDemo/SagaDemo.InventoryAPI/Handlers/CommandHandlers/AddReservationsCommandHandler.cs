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
    public class AddReservationsCommandHandler : IAddReservationsCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IInventoryBatchCommandValidator<AddReservationsCommand> requestValidator;

        public AddReservationsCommandHandler(IDocumentStore documentStore, IInventoryBatchCommandValidator<AddReservationsCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddReservationsCommand command, CancellationToken cancellationToken)
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
                    // Ignore, retry until either successfully updated or validation detects that there are not enough unreserved stocks.
                }
            }
        }

        private async Task HandleInternalAsync(AddReservationsCommand command, CancellationToken cancellationToken)
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

                    loadedProduct.ReservationCount += productQuantityLookup[pair.Key];

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}