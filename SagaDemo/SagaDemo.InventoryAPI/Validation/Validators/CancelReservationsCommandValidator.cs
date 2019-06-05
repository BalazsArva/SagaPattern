using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class CancelReservationsCommandValidator : AbstractValidator<CancelReservationsCommand>
    {
        public CancelReservationsCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.TransactionIdRequired);
        }
    }
}