using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class CompleteDeliveryCommandValidator : AbstractValidator<CompleteDeliveryCommand>
    {
        public CompleteDeliveryCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}