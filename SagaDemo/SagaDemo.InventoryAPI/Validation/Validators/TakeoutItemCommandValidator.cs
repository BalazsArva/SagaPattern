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
                            var product = validationContext.GetProductFromLookup(command.ProductId);

                            return product.StockCount >= quantity;
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsAvailable);

                    RuleFor(x => x.Quantity)
                        .Must((command, quantity, validationContext) =>
                        {
                            var product = validationContext.GetProductFromLookup(command.ProductId);

                            return product.ReservationCount >= quantity;
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsReservations);
                });
        }
    }
}