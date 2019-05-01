using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddStocksCommandValidator : InventoryBatchCommandValidatorBase<AddStocksCommand>
    {
        public AddStocksCommandValidator(IValidator<AddStockCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }
    }
}