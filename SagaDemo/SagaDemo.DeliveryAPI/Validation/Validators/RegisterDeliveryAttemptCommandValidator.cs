using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class RegisterDeliveryAttemptCommandValidator : AbstractValidator<RegisterDeliveryAttemptCommand>
    {
        public RegisterDeliveryAttemptCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}