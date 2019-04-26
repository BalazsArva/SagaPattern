using System;
using System.Threading;
using System.Threading.Tasks;
using SagaDemo.LoyaltyPointsAPI.DataAccess;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public class RefundPointsCommandHandler : ICommandHandler<RefundPointsCommand>
    {
        private const string Reason = "RefundPoints";

        private readonly ILoyaltyDbContextFactory dbContextFactory;

        public RefundPointsCommandHandler(ILoyaltyDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task HandleAsync(RefundPointsCommand command, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                context.PointsChangedEvents.Add(new PointsChangedEvent
                {
                    PointChange = command.Points,
                    Reason = Reason,
                    UtcDateTimeRecorded = DateTime.UtcNow,
                    UserId = command.UserId
                });

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}