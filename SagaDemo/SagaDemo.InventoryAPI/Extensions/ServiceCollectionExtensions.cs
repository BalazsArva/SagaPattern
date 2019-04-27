using Microsoft.Extensions.DependencyInjection;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services
                .AddSingleton<ICreateProductCommandHandler, CreateProductCommandHandler>();
        }
    }
}