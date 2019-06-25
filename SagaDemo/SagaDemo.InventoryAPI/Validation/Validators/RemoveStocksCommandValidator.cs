using System;
using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.DataAccess.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class RemoveStocksCommandValidator : AbstractValidator<RemoveStocksCommand>, IRemoveStocksCommandValidator
    {
        public RemoveStocksCommandValidator(IValidator<RemoveStockCommand> childItemValidator)
        {
            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.TransactionIdRequired);
        }

        public void ValidateAndThrow(RemoveStocksCommand command, IDictionary<int, Product> productLookup, IDictionary<int, int> availableCountLookup)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (productLookup == null)
            {
                throw new ArgumentNullException(nameof(productLookup));
            }

            if (availableCountLookup == null)
            {
                throw new ArgumentNullException(nameof(availableCountLookup));
            }

            var context = new ValidationContext<RemoveStocksCommand>(command)
            {
                RootContextData =
                {
                    [ValidationContextKeys.Products] = productLookup,
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