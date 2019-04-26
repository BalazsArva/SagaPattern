using System;
using Microsoft.EntityFrameworkCore;

namespace SagaDemo.LoyaltyPointsAPI.DataAccess
{
    public class LoyaltyDbContextFactory : ILoyaltyDbContextFactory
    {
        private readonly DbContextOptions<LoyaltyDbContext> dbContextOptions;

        public LoyaltyDbContextFactory(DbContextOptions<LoyaltyDbContext> dbContextOptions)
        {
            this.dbContextOptions = dbContextOptions ?? throw new ArgumentNullException(nameof(dbContextOptions));
        }

        public LoyaltyDbContext CreateDbContext()
        {
            return new LoyaltyDbContext(dbContextOptions);
        }
    }
}