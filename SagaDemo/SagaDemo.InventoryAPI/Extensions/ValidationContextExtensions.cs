using System;
using System.Collections.Generic;
using FluentValidation.Validators;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Validation;

namespace SagaDemo.InventoryAPI.Extensions
{
    public static class ValidationContextExtensions
    {
        public static IDictionary<int, Product> GetProductLookup(this PropertyValidatorContext context)
        {
            if (context.ParentContext.RootContextData[ValidationContextKeys.Products] is IDictionary<int, Product> productLookup)
            {
                return productLookup;
            }

            throw new InvalidOperationException("Could not find the product lookup in the validation context.");
        }

        public static IDictionary<int, ProductReservation> GetReservationLookup(this PropertyValidatorContext context)
        {
            if (context.ParentContext.RootContextData[ValidationContextKeys.Reservations] is IDictionary<int, ProductReservation> reservationLookup)
            {
                return reservationLookup;
            }

            throw new InvalidOperationException("Could not find the reservation lookup in the validation context.");
        }

        public static IDictionary<int, int> GetAvailableCountLookup(this PropertyValidatorContext context)
        {
            if (context.ParentContext.RootContextData[ValidationContextKeys.AvailableCounts] is IDictionary<int, int> availableCountLookup)
            {
                return availableCountLookup;
            }

            throw new InvalidOperationException("Could not find the available count lookup in the validation context.");
        }

        public static Product GetProductFromLookup(this PropertyValidatorContext context, int productId)
        {
            var productLookup = context.GetProductLookup();
            if (productLookup.ContainsKey(productId))
            {
                return productLookup[productId];
            }

            return null;
        }

        public static ProductReservation GetProductReservationFromLookup(this PropertyValidatorContext context, int productId)
        {
            var reservationLookup = context.GetReservationLookup();
            if (reservationLookup.ContainsKey(productId))
            {
                return reservationLookup[productId];
            }

            return null;
        }

        public static bool TryGetAvailableCountFromLookup(this PropertyValidatorContext context, int productId, out int availableCount)
        {
            var availableCountLookup = context.GetAvailableCountLookup();
            if (availableCountLookup.ContainsKey(productId))
            {
                availableCount = availableCountLookup[productId];

                return true;
            }

            availableCount = 0;

            return false;
        }
    }
}