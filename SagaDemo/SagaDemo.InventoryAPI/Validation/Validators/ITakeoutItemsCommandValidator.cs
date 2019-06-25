using System.Collections.Generic;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public interface ITakeoutItemsCommandValidator
    {
        void ValidateAndThrow(TakeoutItemsCommand command, IDictionary<int, Product> productLookup, IDictionary<int, ProductReservation> reservationLookup, IDictionary<int, int> availableCountLookup);
    }
}