using System;
using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class TakeoutItemsCommandValidator : AbstractValidator<TakeoutItemsCommand>, ITakeoutItemsCommandValidator
    {
        public TakeoutItemsCommandValidator(IValidator<TakeoutItemCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.TransactionIdRequired);
        }

        public void ValidateAndThrow(TakeoutItemsCommand command, IDictionary<int, Product> productLookup, IDictionary<int, ProductReservation> reservationLookup, IDictionary<int, int> availableCountLookup)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (productLookup == null)
            {
                throw new ArgumentNullException(nameof(productLookup));
            }

            if (reservationLookup == null)
            {
                throw new ArgumentNullException(nameof(reservationLookup));
            }

            if (availableCountLookup == null)
            {
                throw new ArgumentNullException(nameof(availableCountLookup));
            }

            var context = new ValidationContext<TakeoutItemsCommand>(command)
            {
                RootContextData =
                {
                    [ValidationContextKeys.Products] = productLookup,
                    [ValidationContextKeys.Reservations] = reservationLookup,
                    [ValidationContextKeys.AvailableCounts] = availableCountLookup
                }
            };

            var validationResult = Validate(context);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}