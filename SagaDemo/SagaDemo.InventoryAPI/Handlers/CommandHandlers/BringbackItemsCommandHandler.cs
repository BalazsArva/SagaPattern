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
    public class BringbackItemsCommandHandler : CommandHandlerBase, IBringbackItemsCommandHandler
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
                var productIds = command.Items.Select(i => i.ProductId).ToList();

                var productLookup = await GetProductLookupAsync(context, productIds, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup);

                // This is for idempotence. We check only the TransactionId because we assume that if one item in a transaction is brought back then so are the others.
                var itemsAlreadyBroughtBack = await context.ProductBroughtBackEvents.AnyAsync(evt => evt.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (itemsAlreadyBroughtBack)
                {
                    return;
                }

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
    }
}