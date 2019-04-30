using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class CancelDeliveryCommandValidator : AbstractValidator<CancelDeliveryCommand>
    {
        public CancelDeliveryCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}