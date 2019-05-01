using FluentValidation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddReservationsCommandValidator : InventoryBatchCommandValidatorBase<AddReservationsCommand>
    {
        public AddReservationsCommandValidator(IValidator<AddReservationCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }
    }
}