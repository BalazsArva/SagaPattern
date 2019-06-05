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
    public class RemoveStocksCommandHandler : IRemoveStocksCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IRemoveStocksCommandValidator requestValidator;

        public RemoveStocksCommandHandler(IInventoryDbContextFactory dbContextFactory, IRemoveStocksCommandValidator requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(RemoveStocksCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productLookup = await GetProductLookupAsync(context, command, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                foreach (var removedStock in command.Items)
                {
                    context.ProductStockRemovedEvents.Add(new ProductStockRemovedEvent
                    {
                        ProductId = removedStock.ProductId,
                        Quantity = removedStock.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, RemoveStocksCommand command, CancellationToken cancellationToken)
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