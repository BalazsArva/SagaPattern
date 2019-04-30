using FluentValidation;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class CreateDeliveryRequestCommandValidator : AbstractValidator<CreateDeliveryRequestCommand>
    {
        public CreateDeliveryRequestCommandValidator()
        {
            // TODO: Implement rules
        }
    }
}