using System;
using System.Collections.Generic;
using FluentValidation.Validators;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Validation;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ValidationContextExtensions
    {
        public static IDictionary<string, Product> GetProductLookup(this PropertyValidatorContext validationContext)
        {
            if (validationContext.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<string, Product> productLookup)
            {
                return productLookup;
            }

            throw new InvalidOperationException("Could not find the product lookup in the validation context.");
        }

        public static Product GetProductFromLookup(this PropertyValidatorContext validationContext, string productId)
        {
            var productLookup = validationContext.GetProductLookup();

            if (productLookup.ContainsKey(productId))
            {
                return productLookup[productId];
            }

            return null;
        }
    }
}