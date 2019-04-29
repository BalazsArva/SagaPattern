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
    public class AddReservationsCommandHandler : IAddReservationsCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<AddReservationsCommand> requestValidator;

        public AddReservationsCommandHandler(IDocumentStore documentStore, IValidator<AddReservationsCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddReservationsCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                var productCommandLookup = command.Reservations.ToDictionary(s => s.ProductId);
                var productLookup = await session.LoadProductsAsync(productCommandLookup.Keys, cancellationToken).ConfigureAwait(false);

                foreach (var pair in productLookup)
                {
                    var loadedProduct = pair.Value;

                    var changeVector = session.Advanced.GetChangeVectorFor(loadedProduct);
                    var productReservation = productCommandLookup[pair.Key];

                    loadedProduct.ReservationCount += productReservation.Quantity;

                    await session.StoreAsync(loadedProduct, changeVector, loadedProduct.Id, cancellationToken).ConfigureAwait(false);
                }

                // TODO: Catch concurrency exception
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}