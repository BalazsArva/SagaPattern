using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class TakeoutItemsCommandValidator : InventoryBatchCommandValidatorBase<TakeoutItemsCommand>
    {
        public TakeoutItemsCommandValidator(IValidator<TakeoutItemCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }
    }
}