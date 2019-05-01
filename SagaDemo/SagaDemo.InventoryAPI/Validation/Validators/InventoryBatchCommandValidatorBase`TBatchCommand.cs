using System;
using System.Collections.Generic;
using FluentValidation;
using SagaDemo.InventoryAPI.Entities;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public abstract class InventoryBatchCommandValidatorBase<TBatchCommand> : AbstractValidator<TBatchCommand>, IInventoryBatchCommandValidator<TBatchCommand>
        where TBatchCommand : class
    {
        public virtual void ValidateAndThrow(TBatchCommand command, IDictionary<string, Product> productLookup)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (productLookup == null)
            {
                throw new ArgumentNullException(nameof(productLookup));
            }

            var context = new ValidationContext<TBatchCommand>(command)
            {
                RootContextData =
                {
                    [ValidationContextKeys.Products] = productLookup
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