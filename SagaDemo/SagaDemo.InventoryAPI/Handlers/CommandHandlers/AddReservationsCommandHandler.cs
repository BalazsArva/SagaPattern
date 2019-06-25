using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Validation.Validators;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class AddReservationsCommandHandler : CommandHandlerBase, IAddReservationsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IAddReservationsCommandValidator requestValidator;

        public AddReservationsCommandHandler(IInventoryDbContextFactory dbContextFactory, IAddReservationsCommandValidator requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddReservationsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productIds = command.Items.Select(i => i.ProductId).ToList();

                var productLookup = await GetProductLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                // This is for idempotence. We check only the TransactionId because we assume that if one item in a transaction is reserved then so are the others.
                var alreadyReserved = await context.ProductReservations.AnyAsync(r => r.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (alreadyReserved)
                {
                    return;
                }

                foreach (var addedReservation in command.Items)
                {
                    context.ProductReservations.Add(new ProductReservation
                    {
                        ProductId = addedReservation.ProductId,
                        Quantity = addedReservation.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}