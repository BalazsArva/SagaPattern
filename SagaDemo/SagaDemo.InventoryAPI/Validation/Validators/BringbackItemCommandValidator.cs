using System;
using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class BringbackItemCommandValidator : AbstractValidator<BringbackItemCommand>
    {
        public BringbackItemCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .Must((command, productId, validationContext) =>
                {
                    if (validationContext.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<string, Product> productLookup)
                    {
                        return productLookup.ContainsKey(productId) && productLookup[productId] != null;
                    }

                    throw new InvalidOperationException("Could not find the product lookup in the validation context.");
                })
                .WithMessage(ValidationMessages.ProductDoesNotExist)
                .DependentRules(() =>
                {
                    RuleFor(x => x.Quantity)
                        .GreaterThan(0)
                        .WithMessage(ValidationMessages.QuantityMustBePositive);
                });
        }
    }
}