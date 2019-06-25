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
    public class AddStocksCommandHandler : CommandHandlerBase, IAddStocksCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IAddStocksCommandValidator requestValidator;

        public AddStocksCommandHandler(IInventoryDbContextFactory dbContextFactory, IAddStocksCommandValidator requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddStocksCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productIds = command.Items.Select(i => i.ProductId).ToList();

                var productLookup = await GetProductLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                // This is for idempotence. We check only the TransactionId because we assume that if stocks for one product in a transaction is added then so are the others.
                var stocksAlreadyAdded = await context.ProductStockAddedEvents.AnyAsync(evt => evt.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (stocksAlreadyAdded)
                {
                    return;
                }

                foreach (var addedStock in command.Items)
                {
                    context.ProductStockAddedEvents.Add(new ProductStockAddedEvent
                    {
                        ProductId = addedStock.ProductId,
                        Quantity = addedStock.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}