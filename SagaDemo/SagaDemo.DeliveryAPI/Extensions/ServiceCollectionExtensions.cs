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
                .AddSingleton<ICreateDeliveryRequestCommandHandler, CreateDeliveryRequestCommandHandler>()
                .AddSingleton<IRegisterDeliveryAttemptCommandHandler, RegisterDeliveryAttemptCommandHandler>()
                .AddSingleton<ICompleteDeliveryCommandHandler, CompleteDeliveryCommandHandler>()
                .AddSingleton<ICancelDeliveryCommandHandler, CancelDeliveryCommandHandler>();

            services
                .AddSingleton<IValidator<Address>, AddressValidator>()
                .AddSingleton<IValidator<CreateDeliveryRequestCommand>, CreateDeliveryRequestCommandValidator>()
                .AddSingleton<IValidator<RegisterDeliveryAttemptCommand>, RegisterDeliveryAttemptCommandValidator>()
                .AddSingleton<IValidator<CompleteDeliveryCommand>, CompleteDeliveryCommandValidator>()
                .AddSingleton<IValidator<CancelDeliveryCommand>, CancelDeliveryCommandValidator>();

            return services;
        }
    }
}