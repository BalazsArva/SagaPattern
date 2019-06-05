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
    public class BringbackItemsCommandHandler : IBringbackItemsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IInventoryBatchCommandValidator<BringbackItemsCommand> requestValidator;

        public BringbackItemsCommandHandler(IInventoryDbContextFactory dbContextFactory, IInventoryBatchCommandValidator<BringbackItemsCommand> requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(BringbackItemsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productLookup = await GetProductLookupAsync(context, command, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                foreach (var broughtBackItem in command.Items)
                {
                    context.ProductBroughtBackEvents.Add(new ProductBroughtBackEvent
                    {
                        ProductId = broughtBackItem.ProductId,
                        Quantity = broughtBackItem.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, BringbackItemsCommand command, CancellationToken cancellationToken)
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