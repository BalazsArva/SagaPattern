using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Validation
{
    public class EarnPointsCommandValidator : AbstractValidator<EarnPointsCommand>
    {
        public EarnPointsCommandValidator(ILoyaltyDbContextFactory dbContextFactory)
        {
            RuleFor(cmd => cmd.Points)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.PointsMustBePositive);

            RuleFor(cmd => cmd.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(cmd => cmd.UserId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}