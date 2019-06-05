using FluentValidation;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class TakeoutItemCommandValidator : AbstractValidator<TakeoutItemCommand>
    {
        public TakeoutItemCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .Must((command, productId, validationContext) =>
                {
                    return validationContext.GetProductFromLookup(productId) != null;
                })
                .WithMessage(ValidationMessages.ProductDoesNotExist)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage(ValidationMessages.QuantityMustBePositive);

                    RuleFor(x => x.Quantity)
                        .Must((command, quantity, validationContext) =>
                        {
                            // TODO: Validate that appropriate reservations exist (using the transaction id). Remove the available stocks validation from the reservation validator, since that is only a reservation. Availability must be validated at takeout.
                            return true;
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsAvailable);

                    RuleFor(x => x.Quantity)
                        .Must((command, quantity, validationContext) =>
                        {
                            // TODO: Validate that appropriate reservations exist (using the transaction id).
                            return true;
                        })
                        .When(cmd => cmd.Quantity > 0)
                        // TODO: Change message to match new semantics
                        .WithMessage(ValidationMessages.QuantityExceedsReservations);
                });
        }
    }
}