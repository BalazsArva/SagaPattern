using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class ConsumePointsCommandHandler : ICommandHandler<ConsumePointsCommand>
    {
        private const string ConsumePointsReason = "ConsumePoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;

        public ConsumePointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task HandleAsync(ConsumePointsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var totalPoints = await context.PointsChangedEvents.SumAsync(evt => evt.PointChange).ConfigureAwait(false);

                if (totalPoints < command.Points)
                {
                    // TODO: Create and throw custom error
                }

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