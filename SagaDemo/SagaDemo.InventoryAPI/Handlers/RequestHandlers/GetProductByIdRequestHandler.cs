using System;
using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.Operations.Requests;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.RequestHandlers
{
    public class GetProductByIdRequestHandler : IGetProductByIdRequestHandler
    {
        private readonly IInventoryDbContextFactory dbContextFactory;

        public GetProductByIdRequestHandler(IInventoryDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<GetProductByIdResponse> HandleAsync(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var product = await context.Products.FindAsync(new[] { request.ProductId }, cancellationToken).ConfigureAwait(false);

                if (product == null)
                {
                    return null;
                }

                return new GetProductByIdResponse
                {
                    Name = product.Name,
                    ProductId = product.Id,
                    PointsCost = product.PointsCost
                };
            }
        }
    }
}