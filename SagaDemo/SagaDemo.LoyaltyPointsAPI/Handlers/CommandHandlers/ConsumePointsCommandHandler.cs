using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class ConsumePointsCommandHandler : ICommandHandler<ConsumePointsCommand>
    {
        private const string ConsumePointsReason = "ConsumePoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;
        private readonly IValidator<ConsumePointsCommand> commandValidator;

        public ConsumePointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory, IValidator<ConsumePointsCommand> commandValidator)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            this.commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        }

        public async Task HandleAsync(ConsumePointsCommand command, CancellationToken cancellationToken)
        {
            await commandValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var context = dbContextFactory.CreateDbContext())
            {
                context.PointsChangedEvents.Add(new PointsChangedEvent
                {
                    PointChange = -command.Points,
                    Reason = ConsumePointsReason,
                    UtcDateTimeRecorded = DateTime.UtcNow,
                    UserId = command.UserId,
                    TransactionId = command.TransactionId
                });

                // Assume optimistic concurrency so that points aren't changed between retrieving the sum and adding the consume record.
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}