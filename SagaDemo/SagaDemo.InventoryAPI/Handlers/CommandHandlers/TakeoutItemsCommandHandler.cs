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
        private readonly IInventoryBatchCommandValidator<TakeoutItemsCommand> requestValidator;

        public TakeoutItemsCommandHandler(IInventoryDbContextFactory dbContextFactory, IInventoryBatchCommandValidator<TakeoutItemsCommand> requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productLookup = await GetProductLookupAsync(context, command, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

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

        private static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, TakeoutItemsCommand command, CancellationToken cancellationToken)
        {
            var productIds = command.Items.Select(i => i.ProductId);

            var products = await context
                .Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return products.ToDictionary(p => p.Id);
        }
    }
}