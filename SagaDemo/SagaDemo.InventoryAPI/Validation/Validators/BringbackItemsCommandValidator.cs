using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class BringbackItemsCommandValidator : InventoryBatchCommandValidatorBase<BringbackItemsCommand>
    {
        public BringbackItemsCommandValidator(IValidator<BringbackItemCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.TransactionIdRequired);
        }
    }
}