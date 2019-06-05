using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class CancelReservationsCommandHandler : ICancelReservationsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IValidator<CancelReservationsCommand> requestValidator;

        public CancelReservationsCommandHandler(IInventoryDbContextFactory dbContextFactory, IValidator<CancelReservationsCommand> requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(CancelReservationsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                // TODO: The validator should check that the reservations actually exist
                // But also should ensure that this operation is idempotent so multiple cancel attempts should succeed (with duplicated ones being ignored).
                requestValidator.ValidateAndThrow(command);

                var productReservations = await context
                    .ProductReservationAddedEvents
                    .Where(evt => evt.TransactionId == command.TransactionId)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var productReservation in productReservations)
                {
                    context.ProductReservationCancelledEvents.Add(new ProductReservationCancelledEvent
                    {
                        ProductId = productReservation.ProductId,
                        Quantity = productReservation.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}