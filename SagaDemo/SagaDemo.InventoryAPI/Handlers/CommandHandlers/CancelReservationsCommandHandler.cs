using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess;
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
                requestValidator.ValidateAndThrow(command);

                var productReservations = await context
                    .ProductReservations
                    .Where(evt => evt.TransactionId == command.TransactionId)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                context.ProductReservations.RemoveRange(productReservations);

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}