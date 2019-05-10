using System.Threading;
using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;
using SagaDemo.LoyaltyPointsAPI.Utilities;

namespace SagaDemo.LoyaltyPointsAPI.Validation.Validators
{
    public class ConsumePointsCommandValidator : AbstractValidator<ConsumePointsCommand>
    {
        public ConsumePointsCommandValidator(IPointsBalanceCalculator pointsBalanceCalculator)
        {
            RuleFor(cmd => cmd.Points)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.PointsMustBePositive)
                .DependentRules(() =>
                {
                    RuleFor(cmd => cmd.Points)
                        .MustAsync(async (ConsumePointsCommand command, int points, CancellationToken cancellationToken) =>
                        {
                            var total = await pointsBalanceCalculator.CalculateTotalAsync(command.UserId, cancellationToken).ConfigureAwait(false);

                            return total >= points;
                        })
                        .WithMessage(ValidationMessages.ConsumedPointsExceedTotal);
                });

            RuleFor(cmd => cmd.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(cmd => cmd.UserId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}