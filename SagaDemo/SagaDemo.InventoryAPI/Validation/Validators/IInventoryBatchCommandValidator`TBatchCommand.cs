using System.Collections.Generic;
using SagaDemo.InventoryAPI.Entities;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public interface IInventoryBatchCommandValidator<TBatchCommand>
    {
        void ValidateAndThrow(TBatchCommand command, IDictionary<string, Product> productLookup);
    }
}