using Microsoft.EntityFrameworkCore;
using SagaDemo.LoyaltyPointsAPI.DataAccess.Entities;

namespace SagaDemo.LoyaltyPointsAPI.DataAccess
{
    public class LoyaltyDbContext : DbContext
    {
        public LoyaltyDbContext(DbContextOptions<LoyaltyDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<PointsChangedEvent> PointsChangedEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PointsChangedEvent>()
                .HasKey(e => e.Id)
                .ForSqlServerIsClustered();

            modelBuilder.Entity<PointsChangedEvent>()
                .Property(e => e.Id)
                .UseSqlServerIdentityColumn();

            modelBuilder.Entity<PointsChangedEvent>()
                .Property(e => e.Reason)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder.Entity<PointsChangedEvent>()
                .Property(e => e.TransactionId)
                .HasMaxLength(256)
                .IsRequired(true);
        }
    }
}