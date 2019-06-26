using Microsoft.Extensions.DependencyInjection;
using SagaDemo.OrderAPI.Providers;

namespace SagaDemo.OrderAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrderApiServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IGuidProvider, GuidProvider>();
        }
    }
}