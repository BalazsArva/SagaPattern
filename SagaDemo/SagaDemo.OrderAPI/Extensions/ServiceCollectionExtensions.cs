using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.Exceptions;
using Raven.Client.Exceptions.Database;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using SagaDemo.DeliveryAPI.ApiClient;
using SagaDemo.InventoryAPI.ApiClient;
using SagaDemo.LoyaltyPointsAPI.ApiClient;
using SagaDemo.OrderAPI.Providers;
using SagaDemo.OrderAPI.Services.Handlers.CommandHandlers;

namespace SagaDemo.OrderAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public const int RetryCount = 10;
        public const int RetryDelay = 3000;

        public const string DeliveryApiBaseUrl = "http://localhost:5001";
        public const string InventoryApiBaseUrl = "http://localhost:5002";
        public const string LoyaltyPointsApiBaseUrl = "http://localhost:5003";

        public static IServiceCollection AddOrderApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGuidProvider, GuidProvider>();
            services.AddSingleton<IRegisterOrderCommandHandler, RegisterOrderCommandHandler>();
            services.AddRavenDb(configuration);

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

        private static IServiceCollection AddRavenDb(this IServiceCollection services, IConfiguration configuration)
        {
            var ravenDbUrls = configuration.GetSection("DataAccess:RavenDb:Urls").Get<string[]>();
            var ravenDbDatabase = configuration["DataAccess:RavenDb:Database"];

            EnsureDatabaseExists(ravenDbUrls, ravenDbDatabase);

            var store = new DocumentStore
            {
                Urls = ravenDbUrls,
                Database = ravenDbDatabase
            }.Initialize();

            services.AddSingleton(store);

            return services;
        }

        private static void EnsureDatabaseExists(string[] urls, string database)
        {
            if (string.IsNullOrWhiteSpace(database))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));
            }

            var store = new DocumentStore { Urls = urls }.Initialize();

            try
            {
                store.Maintenance.ForDatabase(database).Send(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                try
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(database), 3));
                }
                catch (ConcurrencyException)
                {
                    // The database was already created before calling CreateDatabaseOperation
                }
            }
        }
    }
}