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
    public class TakeoutItemsCommandHandler : ITakeoutItemsCommandHandler
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

        private static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, IEnumerable<int> productIds, CancellationToken cancellationToken)
        {
            var products = await context
                .Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return products.ToDictionary(p => p.Id);
        }

        private static async Task<IDictionary<int, ProductReservation>> GetReservationLookupAsync(InventoryDbContext context, string transactionId, CancellationToken cancellationToken)
        {
            var reservations = await context.ProductReservations.AsNoTracking().Where(r => r.TransactionId == transactionId).ToListAsync(cancellationToken).ConfigureAwait(false);

            return reservations.ToDictionary(r => r.ProductId);
        }

        private static async Task<IDictionary<int, int>> GetAvailabileCountLookupAsync(InventoryDbContext context, IEnumerable<int> productIds, CancellationToken cancellationToken)
        {
            var addedStocks = context
                .ProductStockAddedEvents
                .Select(e => new { e.ProductId, e.Quantity });

            var removedStocks = context
                .ProductStockRemovedEvents
                .Select(e => new { e.ProductId, Quantity = -e.Quantity });

            var itemsTakenOut = context
                .ProductTakenOutEvents
                .Select(e => new { e.ProductId, Quantity = -e.Quantity });

            var itemsBroughtBack = context
                .ProductBroughtBackEvents
                .Select(e => new { e.ProductId, e.Quantity });

            var allStockChanges = await addedStocks
                .Concat(removedStocks)
                .Concat(itemsTakenOut)
                .Concat(itemsBroughtBack)
                .Where(evt => productIds.Contains(evt.ProductId))
                .GroupBy(s => s.ProductId, (key, elements) => new { ProductId = key, Quantity = elements.Sum(e => e.Quantity) })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return allStockChanges.ToDictionary(grp => grp.ProductId, grp => grp.Quantity);
        }
    }
}