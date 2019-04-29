﻿using System;
using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.Entities;
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

                    RuleFor(x => x.Quantity)
                        .Must((command, quantity, validationContext) =>
                        {
                            if (validationContext.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<string, Product> productLookup)
                            {
                                return productLookup[command.ProductId].StockCount >= quantity;
                            }

                            throw new InvalidOperationException("Could not find the product lookup in the validation context.");
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsAvailable);

                    RuleFor(x => x.Quantity)
                        .Must((command, quantity, validationContext) =>
                        {
                            if (validationContext.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<string, Product> productLookup)
                            {
                                return productLookup[command.ProductId].ReservationCount >= quantity;
                            }

                            throw new InvalidOperationException("Could not find the product lookup in the validation context.");
                        })
                        .When(cmd => cmd.Quantity > 0)
                        .WithMessage(ValidationMessages.QuantityExceedsReservations);
                });
        }
    }
}