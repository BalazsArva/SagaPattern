using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public interface IDeliveryCommandValidator<TCommand>
        where TCommand : IDeliveryCommand
    {
        void ValidateAndThrow(TCommand command, Delivery deliveryDocument);
    }
}