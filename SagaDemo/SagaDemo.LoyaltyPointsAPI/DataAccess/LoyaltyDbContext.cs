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

        public DbSet<PointsConsumedEvent> PointsConsumedEvents { get; set; }

        public DbSet<PointsRefundedEvent> PointsRefundedEvents { get; set; }

        public DbSet<PointsEarnedEvent> PointsEarnedEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: Extract common properties of these entities into a base class
            SetupPointsConsumedEvent(modelBuilder);
            SetupPointsEarnedEvent(modelBuilder);
            SetupPointsRefundedEvent(modelBuilder);
        }

        private void SetupPointsEarnedEvent(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PointsEarnedEvent>()
                .HasKey(e => e.Id)
                .ForSqlServerIsClustered();

            modelBuilder
                .Entity<PointsEarnedEvent>()
                .Property(e => e.Id)
                .UseSqlServerIdentityColumn();

            modelBuilder
                .Entity<PointsEarnedEvent>()
                .Property(e => e.Reason)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsEarnedEvent>()
                .Property(e => e.TransactionId)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsEarnedEvent>()
                .ForSqlServerHasIndex(evt => evt.UserId)
                .IsUnique(false);

            modelBuilder
                .Entity<PointsEarnedEvent>()
                .ForSqlServerHasIndex(evt => evt.TransactionId)
                .IsUnique(false);
        }

        private void SetupPointsConsumedEvent(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PointsConsumedEvent>()
                .HasKey(e => e.Id)
                .ForSqlServerIsClustered();

            modelBuilder
                .Entity<PointsConsumedEvent>()
                .Property(e => e.Id)
                .UseSqlServerIdentityColumn();

            modelBuilder
                .Entity<PointsConsumedEvent>()
                .Property(e => e.Reason)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsConsumedEvent>()
                .Property(e => e.TransactionId)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsConsumedEvent>()
                .ForSqlServerHasIndex(evt => evt.UserId)
                .IsUnique(false);

            modelBuilder
                .Entity<PointsConsumedEvent>()
                .ForSqlServerHasIndex(evt => evt.TransactionId)
                .IsUnique(false);
        }

        private void SetupPointsRefundedEvent(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PointsRefundedEvent>()
                .HasKey(e => e.Id)
                .ForSqlServerIsClustered();

            modelBuilder
                .Entity<PointsRefundedEvent>()
                .Property(e => e.Id)
                .UseSqlServerIdentityColumn();

            modelBuilder
                .Entity<PointsRefundedEvent>()
                .Property(e => e.Reason)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsRefundedEvent>()
                .Property(e => e.TransactionId)
                .HasMaxLength(256)
                .IsRequired(true);

            modelBuilder
                .Entity<PointsRefundedEvent>()
                .ForSqlServerHasIndex(evt => evt.UserId)
                .IsUnique(false);

            modelBuilder
                .Entity<PointsRefundedEvent>()
                .ForSqlServerHasIndex(evt => evt.TransactionId)
                .IsUnique(false);
        }
    }
}