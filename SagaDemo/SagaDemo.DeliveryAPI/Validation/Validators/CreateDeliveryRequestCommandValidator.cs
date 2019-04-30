using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Operations.Commands;
using SagaDemo.DeliveryAPI.Operations.DataStructures;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class CreateDeliveryRequestCommandValidator : AbstractValidator<CreateDeliveryRequestCommand>
    {
        public CreateDeliveryRequestCommandValidator(IValidator<Address> addressValidator)
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.Address)
                .SetValidator(addressValidator);
        }
    }
}