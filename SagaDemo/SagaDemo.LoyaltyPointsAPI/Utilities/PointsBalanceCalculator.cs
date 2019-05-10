using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SagaDemo.LoyaltyPointsAPI.DataAccess;

namespace SagaDemo.LoyaltyPointsAPI.Utilities
{
    public class PointsBalanceCalculator : IPointsBalanceCalculator
    {
        private readonly ILoyaltyDbContextFactory dbContextFactory;

        public PointsBalanceCalculator(ILoyaltyDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<int> CalculateTotalAsync(int userId, CancellationToken cancellationToken)
        {
            using (var context = dbContextFactory.CreateDbContext())
            {
                var totalEarned = await context.PointsEarnedEvents.Where(evt => evt.UserId == userId).SumAsync(e => e.PointChange).ConfigureAwait(false);
                var totalRefunded = await context.PointsRefundedEvents.Where(evt => evt.UserId == userId).SumAsync(e => e.PointChange).ConfigureAwait(false);
                var totalConsumed = await context.PointsConsumedEvents.Where(evt => evt.UserId == userId).SumAsync(e => e.PointChange).ConfigureAwait(false);

                var total = totalEarned + totalRefunded - totalConsumed;

                return total;
            }
        }
    }
}