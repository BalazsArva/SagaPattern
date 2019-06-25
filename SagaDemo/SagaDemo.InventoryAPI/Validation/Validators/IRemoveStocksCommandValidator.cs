using System.Collections.Generic;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public interface IRemoveStocksCommandValidator
    {
        void ValidateAndThrow(RemoveStocksCommand command, IDictionary<int, Product> productLookup, IDictionary<int, int> availableCountLookup);
    }
}