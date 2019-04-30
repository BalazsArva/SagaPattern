using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public abstract class DeliveryCommandValidatorBase<TCommand> : AbstractValidator<TCommand>, IDeliveryCommandValidator<TCommand>
        where TCommand : IDeliveryCommand
    {
        public const string ObjectRootPath = "";

        public override ValidationResult Validate(ValidationContext<TCommand> context)
        {
            return base.Validate(context);
        }

        public abstract IEnumerable<DeliveryStatus> AllowedOriginStatuses { get; }

        public virtual void ValidateAndThrow(TCommand command, Delivery deliveryDocument)
        {
            if (deliveryDocument == null)
            {
                var failures = new[]
                {
                    new ValidationFailure(nameof(IDeliveryCommand.TransactionId), ValidationMessages.DeliveryDoesNotExist)
                };

                throw new ValidationException(failures);
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