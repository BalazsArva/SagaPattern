using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class RemoveStocksCommandValidator : InventoryBatchCommandValidatorBase<RemoveStocksCommand>, IRemoveStocksCommandValidator
    {
        public RemoveStocksCommandValidator(IValidator<RemoveStockCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }
    }
}