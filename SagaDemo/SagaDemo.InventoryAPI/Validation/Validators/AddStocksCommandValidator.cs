using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddStocksCommandValidator : InventoryBatchCommandValidatorBase<AddStocksCommand>, IAddStocksCommandValidator
    {
        public AddStocksCommandValidator(IValidator<AddStockCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.TransactionIdRequired);
        }
    }
}