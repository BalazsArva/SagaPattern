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
    public class AddReservationsCommandHandler : IAddReservationsCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IAddReservationsCommandValidator requestValidator;

        public AddReservationsCommandHandler(IInventoryDbContextFactory dbContextFactory, IAddReservationsCommandValidator requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(AddReservationsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var productQuantityLookup = command.Items.ToDictionary(cmd => cmd.ProductId, cmd => cmd.Quantity);

                var productLookup = await GetProductLookupAsync(context, command, cancellationToken).ConfigureAwait(false);
                var productReservationsLookup = await GetProductReservationsLookupAsync(context, command, cancellationToken).ConfigureAwait(false);

                requestValidator.ValidateAndThrow(command, productLookup, productReservationsLookup);

                foreach (var pair in productLookup)
                {
                    context.ProductReservationAddedEvents.Add(new ProductReservationAddedEvent
                    {
                        ProductId = pair.Key,
                        Quantity = productQuantityLookup[pair.Key],

                        // TODO: Set this
                        //TransactionId =
                    });
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task<Dictionary<int, int>> GetProductReservationsLookupAsync(InventoryDbContext context, AddReservationsCommand command, CancellationToken cancellationToken)
        {
            var productIds = command.Items.Select(i => i.ProductId);

            var productReservations = await context
                .ProductReservationAddedEvents
                .Where(p => productIds.Contains(p.Id))
                .GroupBy(p => p.Id, (key, values) => new
                {
                    ProductId = key,
                    Quantity = values.Sum(v => v.Quantity)
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return productReservations.ToDictionary(p => p.ProductId, p => p.Quantity);
        }

        private static async Task<IDictionary<int, Product>> GetProductLookupAsync(InventoryDbContext context, AddReservationsCommand command, CancellationToken cancellationToken)
        {
            var productIds = command.Items.Select(i => i.ProductId);

            var products = await context
                .Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return products.ToDictionary(p => p.Id);
        }
    }
}