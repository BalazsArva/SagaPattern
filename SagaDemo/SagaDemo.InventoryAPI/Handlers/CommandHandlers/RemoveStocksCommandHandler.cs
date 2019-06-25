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
    public class RemoveStocksCommandHandler : CommandHandlerBase, IRemoveStocksCommandHandler
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
                var productIds = command.Items.Select(i => i.ProductId).ToList();

                var productLookup = await GetProductLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);
                var availableCountLookup = await GetAvailabileCountLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup, availableCountLookup);

                // This is for idempotence. We check only the TransactionId because we assume that if stocks for one product in a transaction is removed then so are the others.
                var stocksAlreadyRemoved = await context.ProductStockRemovedEvents.AnyAsync(evt => evt.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (stocksAlreadyRemoved)
                {
                    return;
                }

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
    }
}