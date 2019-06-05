using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public interface IRemoveStocksCommandValidator : IInventoryBatchCommandValidator<RemoveStocksCommand>
    {
    }
}