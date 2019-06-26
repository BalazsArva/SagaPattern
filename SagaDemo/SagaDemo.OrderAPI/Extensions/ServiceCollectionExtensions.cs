using System;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using SagaDemo.DeliveryAPI.ApiClient;
using SagaDemo.InventoryAPI.ApiClient;
using SagaDemo.LoyaltyPointsAPI.ApiClient;
using SagaDemo.OrderAPI.Providers;

namespace SagaDemo.OrderAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const int RetryCount = 10;
        public const int RetryDelay = 3000;

        public const string DeliveryApiBaseUrl = "http://localhost:5001";
        public const string InventoryApiBaseUrl = "http://localhost:5002";
        public const string LoyaltyPointsApiBaseUrl = "http://localhost:5003";

        public static IServiceCollection AddOrderApiServices(this IServiceCollection services)
        {
            services.AddSingleton<IGuidProvider, GuidProvider>();

            services.AddHttpClientConsumer<IDeliveryApiClient, DeliveryApiClient>(DeliveryApiBaseUrl);

            services.AddHttpClientConsumer<IReservationsApiClient, ReservationsApiClient>(InventoryApiBaseUrl);
            services.AddHttpClientConsumer<ICatalogApiClient, CatalogApiClient>(InventoryApiBaseUrl);

            services.AddHttpClientConsumer<ILoyaltyPointsApiClient, LoyaltyPointsApiClient>(LoyaltyPointsApiBaseUrl);

            return services;
        }

        private static IHttpClientBuilder AddHttpClientConsumer<TContract, TImplementation>(this IServiceCollection services, string baseUrl)
            where TContract : class
            where TImplementation : class, TContract
        {
            return services
                .AddHttpClient<TContract, TImplementation>(httpClient =>
                {
                    httpClient.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
                })
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(RetryCount, _ => TimeSpan.FromMilliseconds(RetryDelay)));
        }
    }
}