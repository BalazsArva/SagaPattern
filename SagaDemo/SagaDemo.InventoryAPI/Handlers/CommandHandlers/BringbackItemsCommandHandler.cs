using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class BringbackItemsCommandHandler : CommandHandlerBase, IBringbackItemsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IValidator<BringbackItemsCommand> requestValidator;

        public BringbackItemsCommandHandler(IInventoryDbContextFactory dbContextFactory, IValidator<BringbackItemsCommand> requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(BringbackItemsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                requestValidator.ValidateAndThrow(command);

                // This is for idempotence. We check only the TransactionId because we assume that if one item in a transaction is brought back then so are the others.
                var itemsAlreadyBroughtBack = await context.ProductBroughtBackEvents.AnyAsync(evt => evt.TransactionId == command.TransactionId, cancellationToken).ConfigureAwait(false);
                if (itemsAlreadyBroughtBack)
                {
                    return;
                }

                var correspondingTakeoutEvents = await context
                    .ProductTakenOutEvents
                    .AsNoTracking()
                    .Where(evt => evt.TransactionId == command.TransactionId)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (correspondingTakeoutEvents.Count == 0)
                {
                    return;
                }

                foreach (var takeoutEvent in correspondingTakeoutEvents)
                {
                    context.ProductBroughtBackEvents.Add(new ProductBroughtBackEvent
                    {
                        ProductId = takeoutEvent.ProductId,
                        Quantity = takeoutEvent.Quantity,
                        TransactionId = command.TransactionId
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}