using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.DataAccess.Entities;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public abstract class CommandHandlerBase
    {
        protected static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, IEnumerable<int> productIds, CancellationToken cancellationToken)
        {
            var products = await context
                .Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return products.ToDictionary(p => p.Id);
        }

        protected static async Task<IDictionary<int, int>> GetAvailabileCountLookupAsync(InventoryDbContext context, IEnumerable<int> productIds, CancellationToken cancellationToken)
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