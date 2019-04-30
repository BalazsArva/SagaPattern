using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SagaDemo.DeliveryAPI.Handlers.CommandHandlers;
using SagaDemo.DeliveryAPI.Operations.Commands;
using SagaDemo.DeliveryAPI.Operations.DataStructures;
using SagaDemo.DeliveryAPI.Validation.Validators;

namespace SagaDemo.DeliveryAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDeliveryServices(this IServiceCollection services)
        {
            services
                .AddSingleton<ICreateDeliveryRequestCommandHandler, CreateDeliveryRequestCommandHandler>();

            services
                .AddSingleton<IValidator<Address>, AddressValidator>()
                .AddSingleton<IValidator<CreateDeliveryRequestCommand>, CreateDeliveryRequestCommandValidator>();

            return services;
        }
    }
}