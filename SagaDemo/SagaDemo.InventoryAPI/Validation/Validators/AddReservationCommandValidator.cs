using FluentValidation;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddReservationCommandValidator : AbstractValidator<AddReservationCommand>
    {
        public AddReservationCommandValidator()
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
                            var product = validationContext.GetProductFromLookup(command.ProductId);
                            var availableCount = product.StockCount - product.ReservationCount;

                            return availableCount >= quantity;
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsAvailable);
                });
        }
    }
}