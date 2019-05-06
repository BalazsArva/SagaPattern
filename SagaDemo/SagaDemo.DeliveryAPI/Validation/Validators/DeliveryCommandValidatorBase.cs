using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using SagaDemo.Common.Errors;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public abstract class DeliveryCommandValidatorBase<TCommand> : AbstractValidator<TCommand>, IDeliveryCommandValidator<TCommand>
        where TCommand : class, IDeliveryCommand
    {
        public const string ObjectRootPath = "";

        public abstract IEnumerable<DeliveryStatus> AllowedOriginStatuses { get; }

        public virtual void ValidateAndThrow(TCommand command, Delivery deliveryDocument)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (deliveryDocument == null)
            {
                throw new EntityNotFoundException(ValidationMessages.DeliveryDoesNotExist);
            }

            var currentStatus = deliveryDocument.Status;
            if (!AllowedOriginStatuses.Contains(currentStatus))
            {
                var failures = new[]
                {
                    new ValidationFailure(
                        ObjectRootPath,
                        string.Join(
                            "\n",
                            "The status is not in a valid state for this status transition.",
                            "This transition can only be performed if the delivery is in one of the following statuses:",
                            string.Join("\n", AllowedOriginStatuses.Select(s => $"'{s.ToString()}'")),
                            $"The current status is '{currentStatus.ToString()}'"))
                };

                throw new ValidationException(failures);
            }

            this.ValidateAndThrow(command);
        }
    }
}