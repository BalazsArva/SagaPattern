using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Validation.Validators;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services)
        {
            // TODO: Add IDocumentStore
            services
                .AddSingleton<ICreateProductCommandHandler, CreateProductCommandHandler>()
                .AddSingleton<IAddStocksCommandHandler, AddStocksCommandHandler>()
                .AddSingleton<IAddReservationsCommandHandler, AddReservationsCommandHandler>()
                .AddSingleton<IGetProductByIdRequestHandler, GetProductByIdRequestHandler>();

            services
                .AddSingleton<IValidator<AddReservationCommand>, AddReservationCommandValidator>()
                .AddSingleton<IValidator<AddReservationsCommand>, AddReservationsCommandValidator>();

            return services;
        }
    }
}