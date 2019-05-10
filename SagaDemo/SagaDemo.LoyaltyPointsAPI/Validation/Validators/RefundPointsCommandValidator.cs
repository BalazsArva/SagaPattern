using System.Threading;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.Common.Validation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Validation.Validators
{
    public class RefundPointsCommandValidator : AbstractValidator<RefundPointsCommand>
    {
        public RefundPointsCommandValidator(ILoyaltyDbContextFactory dbContextFactory)
        {
            RuleFor(cmd => cmd.TransactionId)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty)
                .DependentRules(() =>
                {
                    RuleFor(cmd => cmd.TransactionId)
                        .MustAsync(async (string transactionId, CancellationToken cancellationToken) =>
                        {
                            using (var context = dbContextFactory.CreateDbContext())
                            {
                                var consumptionEvent = await context
                                    .PointsConsumedEvents
                                    .FirstOrDefaultAsync(e => e.TransactionId == transactionId).ConfigureAwait(false);

                                return consumptionEvent != null;
                            }
                        })
                        .WithMessage(ValidationMessages.PointsConsumptionNotFound);
                });
        }
    }
}