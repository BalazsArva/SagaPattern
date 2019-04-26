using System.Threading;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Validation
{
    public class ConsumePointsCommandValidator : AbstractValidator<ConsumePointsCommand>
    {
        public ConsumePointsCommandValidator(ILoyaltyDbContextFactory dbContextFactory)
        {
            RuleFor(cmd => cmd.Points)
                .MustAsync(async (int points, CancellationToken cancellationToken) =>
                {
                    using (var context = dbContextFactory.CreateDbContext())
                    {
                        var total = await context.PointsChangedEvents.SumAsync(e => e.PointChange).ConfigureAwait(false);

                        return total >= points;
                    }
                })
                .WithMessage(ValidationMessages.ConsumedPointsExceedTotal);

            RuleFor(cmd => cmd.TransactionId)
                .NotEmpty()
                .WithMessage(ValidationMessages.CannotBeNullOrEmpty);

            RuleFor(cmd => cmd.UserId)
                .NotEmpty()
                .WithMessage(ValidationMessages.CannotBeNullOrEmpty);
        }
    }
}