using FluentValidation;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class RemoveStockCommandValidator : AbstractValidator<RemoveStockCommand>
    {
        public RemoveStockCommandValidator()
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
                        .WithMessage(ValidationMessages.QuantityMustBePositive)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Quantity)
                                .Must((command, quantity, context) =>
                                {
                                    if (context.TryGetAvailableCountFromLookup(command.ProductId, out var availableCount))
                                    {
                                        return availableCount >= quantity;
                                    }

                                    return false;
                                });
                        });
                });
        }
    }
}