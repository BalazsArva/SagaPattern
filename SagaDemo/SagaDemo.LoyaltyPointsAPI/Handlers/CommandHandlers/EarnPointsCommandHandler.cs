using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class EarnPointsCommandHandler : ICommandHandler<EarnPointsCommand>
    {
        private const string EarnPointsReason = "EarnPoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;
        private readonly IValidator<EarnPointsCommand> commandValidator;

        public EarnPointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory, IValidator<EarnPointsCommand> commandValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        }

        public async Task HandleAsync(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            await commandValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var context = dbContextFactory.CreateDbContext())
            {
                context.PointsChangedEvents.Add(new PointsChangedEvent
                {
                    PointChange = command.Points,
                    Reason = EarnPointsReason,
                    UtcDateTimeRecorded = DateTime.UtcNow,
                    UserId = command.UserId,
                    TransactionId = command.TransactionId
                });

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}