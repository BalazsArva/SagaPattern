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
                        .WithMessage(ValidationMessages.QuantityMustBePositive)
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Quantity)
                                .Must((command, quantity, context) =>
                                {
                                    var reservationDbEntry = context.GetProductReservationFromLookup(command.ProductId);
                                    if (reservationDbEntry == null)
                                    {
                                        return false;
                                    }

                                    return reservationDbEntry.Quantity == quantity;
                                })
                                .WithMessage(ValidationMessages.QuantityMustMeetReservations);

                            RuleFor(x => x.Quantity)
                                .Must((command, quantity, context) =>
                                {
                                    if (context.TryGetAvailableCountFromLookup(command.ProductId, out var availableCount))
                                    {
                                        return availableCount >= quantity;
                                    }

                                    return false;
                                })
                                .WithMessage(ValidationMessages.QuantityExceedsAvailable);
                        });
                });
        }
    }
}