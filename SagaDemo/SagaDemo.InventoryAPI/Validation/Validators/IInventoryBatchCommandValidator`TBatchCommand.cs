using System.Collections.Generic;
using SagaDemo.InventoryAPI.DataAccess.Entities;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public interface IInventoryBatchCommandValidator<TBatchCommand>
    {
        void ValidateAndThrow(TBatchCommand command, IDictionary<int, Product> productLookup);
    }
}