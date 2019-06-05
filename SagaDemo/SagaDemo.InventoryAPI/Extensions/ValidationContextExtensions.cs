using System;
using System.Collections.Generic;
using FluentValidation.Validators;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Validation;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ValidationContextExtensions
    {
        public static IDictionary<int, Product> GetProductLookup(this PropertyValidatorContext validationContext)
        {
            if (validationContext.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<int, Product> productLookup)
            {
                return productLookup;
            }

            throw new InvalidOperationException("Could not find the product lookup in the validation context.");
        }

        public static Product GetProductFromLookup(this PropertyValidatorContext validationContext, int productId)
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