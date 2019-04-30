using System.Collections.Generic;
using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class CompleteDeliveryCommandValidator : DeliveryCommandValidatorBase<CompleteDeliveryCommand>
    {
        public CompleteDeliveryCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }

        // Allow finished statuses as well to support idempotence
        public override IEnumerable<DeliveryStatus> AllowedOriginStatuses { get; } = new[] { DeliveryStatus.Created, DeliveryStatus.InProgress, DeliveryStatus.Finished };
    }
}