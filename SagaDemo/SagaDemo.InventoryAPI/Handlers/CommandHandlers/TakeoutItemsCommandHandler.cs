using System;
using System.Collections.Generic;
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
    public class TakeoutItemsCommandHandler : CommandHandlerBase, ITakeoutItemsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly ITakeoutItemsCommandValidator requestValidator;

        public TakeoutItemsCommandHandler(IInventoryDbContextFactory dbContextFactory, ITakeoutItemsCommandValidator requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productIds = command.Items.Select(i => i.ProductId).ToList();

                var productLookup = await GetProductLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);
                var reservationLookup = await GetReservationLookupAsync(context, command.TransactionId, cancellationToken).ConfigureAwait(false);
                var availableCountLookup = await GetAvailabileCountLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup, reservationLookup, availableCountLookup);

                // This is for idempotence. We check only the TransactionId because we assume that if one item in a transaction is taken out then so are the others.
                var itemsAlreadyTakenOut = await context.ProductTakenOutEvents.AnyAsync(evt => evt.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (itemsAlreadyTakenOut)
                {
                    return;
                }

                foreach (var takenOutItem in command.Items)
                {
                    context.ProductTakenOutEvents.Add(new ProductTakenOutEvent
                    {
                        ProductId = takenOutItem.ProductId,
                        Quantity = takenOutItem.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<IDictionary<int, ProductReservation>> GetReservationLookupAsync(InventoryDbContext context, string transactionId, CancellationToken cancellationToken)
        {
            var reservations = await context.ProductReservations.AsNoTracking().Where(r => r.TransactionId == transactionId).ToListAsync(cancellationToken).ConfigureAwait(false);

            return reservations.ToDictionary(r => r.ProductId);
        }
    }
}