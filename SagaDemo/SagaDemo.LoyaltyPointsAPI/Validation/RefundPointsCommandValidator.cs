using FluentValidation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Validation
{
    public class RefundPointsCommandValidator : AbstractValidator<RefundPointsCommand>
    {
        public RefundPointsCommandValidator(ILoyaltyDbContextFactory dbContextFactory)
        {
            RuleFor(cmd => cmd.Points)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.PointsMustBePositive);

            RuleFor(cmd => cmd.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.CannotBeNullOrEmpty);

            RuleFor(cmd => cmd.UserId)
                .NotEmpty()
                .WithMessage(ValidationMessages.CannotBeNullOrEmpty);
        }
    }
}