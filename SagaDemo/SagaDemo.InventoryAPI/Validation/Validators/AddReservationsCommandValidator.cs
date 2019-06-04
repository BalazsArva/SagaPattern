using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddReservationsCommandValidator : AbstractValidator<AddReservationsCommand>, IAddReservationsCommandValidator
    {
        public AddReservationsCommandValidator(IValidator<AddReservationCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }

        public void ValidateAndThrow(AddReservationsCommand command, IDictionary<int, Product> productLookup, IDictionary<int, int> reservationQuantityLookup)
        {
            var validationContext = new ValidationContext<AddReservationsCommand>(command)
            {
                RootContextData =
                {
                    [ValidationContextKeys.Products] = productLookup,
                    [ValidationContextKeys.ReservedQuantityLookup] = reservationQuantityLookup
                }
            };

            var validationResult = Validate(validationContext);
            if (!validationResult.IsValid)
            {
                throw new ValidationException("Command validation failed.", validationResult.Errors);
            }
        }
    }
}