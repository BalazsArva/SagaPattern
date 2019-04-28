using Microsoft.Extensions.DependencyInjection;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services
                .AddSingleton<ICreateProductCommandHandler, CreateProductCommandHandler>()
                .AddSingleton<IAddStocksCommandHandler, AddStocksCommandHandler>()
                .AddSingleton<IAddProductRequestsCommandHandler, AddProductRequestsCommandHandler>()
                .AddSingleton<IGetProductByIdRequestHandler, GetProductByIdRequestHandler>();
        }
    }
}