using System;
using System.Threading;
using System.Threading.Tasks;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class EarnPointsCommandHandler : ICommandHandler<EarnPointsCommand>
    {
        private const string EarnPointsReason = "EarnPoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;

        public EarnPointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task HandleAsync(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                context.PointsChangedEvents.Add(new PointsChangedEvent
                {
                    PointChange = command.Points,
                    Reason = EarnPointsReason,
                    UtcDateTimeRecorded = DateTime.UtcNow,
                    UserId = command.UserId
                });

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}