using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.InventoryAPI.DataAccess;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Validation.Validators;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryDb(this IServiceCollection services, IConfiguration configuration)
        {
            var sqlConnectionString = configuration.GetConnectionString("InventoryDb");

            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseSqlServer(sqlConnectionString)
                .Options;

            services.AddSingleton(options);
            services.AddDbContext<InventoryDbContext>(opts => opts.UseSqlServer(sqlConnectionString));
            services.AddSingleton<IInventoryDbContextFactory>(svcProvider =>
            {
                return new InventoryDbContextFactory(options);
            });

            return services;
        }

        public static IServiceCollection AddInventoryServices(this IServiceCollection services)
        {
            services
                .AddSingleton<ICreateProductCommandHandler, CreateProductCommandHandler>()
                .AddSingleton<IAddStocksCommandHandler, AddStocksCommandHandler>()
                .AddSingleton<IRemoveStocksCommandHandler, RemoveStocksCommandHandler>()
                .AddSingleton<IAddReservationsCommandHandler, AddReservationsCommandHandler>()
                .AddSingleton<ITakeoutItemsCommandHandler, TakeoutItemsCommandHandler>()
                .AddSingleton<IBringbackItemsCommandHandler, BringbackItemsCommandHandler>()
                .AddSingleton<IGetProductByIdRequestHandler, GetProductByIdRequestHandler>();

            services
                .AddSingleton<IValidator<CreateProductCommand>, CreateProductCommandValidator>()
                .AddSingleton<IValidator<AddReservationCommand>, AddReservationCommandValidator>()
                .AddSingleton<IValidator<AddStockCommand>, AddStockCommandValidator>()
                .AddSingleton<IValidator<TakeoutItemCommand>, TakeoutItemCommandValidator>()
                .AddSingleton<IValidator<BringbackItemCommand>, BringbackItemCommandValidator>()
                .AddSingleton<IAddReservationsCommandValidator, AddReservationsCommandValidator>()
                .AddSingleton<IAddStocksCommandValidator, AddStocksCommandValidator>()
                .AddSingleton<IRemoveStocksCommandValidator, RemoveStocksCommandValidator>()
                .AddSingleton<IInventoryBatchCommandValidator<TakeoutItemsCommand>, TakeoutItemsCommandValidator>()
                .AddSingleton<IInventoryBatchCommandValidator<BringbackItemsCommand>, BringbackItemsCommandValidator>();

            return services;
        }
    }
}