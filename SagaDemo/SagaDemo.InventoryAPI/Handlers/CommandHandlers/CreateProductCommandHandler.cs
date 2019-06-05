using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class CreateProductCommandHandler : ICreateProductCommandHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;
        private readonly IValidator<CreateProductCommand> requestValidator;

        public CreateProductCommandHandler(IInventoryDbContextFactory dbContextFactory, IValidator<CreateProductCommand> requestValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task<CreateProductResponse> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
        {
            requestValidator.ValidateAndThrow(command);

            using (var context = dbContextFactory.CreateDbContext())
            {
                var product = new Product
                {
                    Name = command.Name,
                    PointsCost = command.PointsCost
                };

                context.Products.Add(product);

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new CreateProductResponse
                {
                    PointsCost = product.PointsCost,
                    ProductId = product.Id,
                    Name = product.Name
                };
            }
        }
    }
}