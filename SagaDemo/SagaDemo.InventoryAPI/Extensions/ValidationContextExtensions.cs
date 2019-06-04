using System;
using System.Collections.Generic;
using FluentValidation.Validators;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Validation;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ValidationContextExtensions
    {
        public static IDictionary<int, int> GetReservedQuantityLookup(this PropertyValidatorContext validationContext)
        {
            if (validationContext.ParentContext.RootContextData[ValidationContextKeys.ReservedQuantityLookup] is IDictionary<int, int> reservedQuantityLookup)
            {
                return reservedQuantityLookup;
            }

            throw new InvalidOperationException("Could not find the reserved quantity lookup in the validation context.");
        }

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

        public static bool TryGetReservedQuantity(this PropertyValidatorContext validationContext, int productId, out int reservedQuantity)
        {
            var reservedQuantityLookup = validationContext.GetReservedQuantityLookup();

            if (reservedQuantityLookup.ContainsKey(productId))
            {
                reservedQuantity = reservedQuantityLookup[productId];
                return true;
            }

            reservedQuantity = default;
            return false;
        }
    }
}