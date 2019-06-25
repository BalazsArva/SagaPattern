using Microsoft.EntityFrameworkCore;
using SagaDemo.InventoryAPI.DataAccess.Entities;

namespace SagaDemo.InventoryAPI.DataAccess
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductReservation> ProductReservations { get; set; }

        public DbSet<ProductStockAddedEvent> ProductStockAddedEvents { get; set; }

        public DbSet<ProductStockRemovedEvent> ProductStockRemovedEvents { get; set; }

        public DbSet<ProductTakenOutEvent> ProductTakenOutEvents { get; set; }

        public DbSet<ProductBroughtBackEvent> ProductBroughtBackEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureProductTable(modelBuilder);
            ConfigureProductReservationsTable(modelBuilder);
            ConfigureProductStockAddedEventsTable(modelBuilder);
            ConfigureProductStockRemovedEventsTable(modelBuilder);
            ConfigureProductTakenOutEventsTable(modelBuilder);
            ConfigureProductBroughtBackEventsTable(modelBuilder);
        }

        private void ConfigureProductTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<Product>();

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.Name).IsRequired(true).HasMaxLength(EntityConstraints.Product.NameMaxLength);
            entityBuilder.Property(x => x.RowVersion).IsRowVersion();
        }

        private void ConfigureProductReservationsTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<ProductReservation>();

            entityBuilder.ForSqlServerHasIndex(x => x.TransactionId).IsUnique(false);

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.TransactionId).IsRequired(true).HasMaxLength(EntityConstraints.TransactionIdMaxLength);
            entityBuilder.HasOne(x => x.Product).WithMany(x => x.Reservations).HasForeignKey(x => x.ProductId);
        }

        private void ConfigureProductStockAddedEventsTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<ProductStockAddedEvent>();

            entityBuilder.ForSqlServerHasIndex(x => x.TransactionId).IsUnique(false);

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.TransactionId).IsRequired(true).HasMaxLength(EntityConstraints.TransactionIdMaxLength);
            entityBuilder.HasOne(x => x.Product).WithMany(x => x.ProductStockAddedEvents).HasForeignKey(x => x.ProductId);
        }

        private void ConfigureProductStockRemovedEventsTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<ProductStockRemovedEvent>();

            entityBuilder.ForSqlServerHasIndex(x => x.TransactionId).IsUnique(false);

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.TransactionId).IsRequired(true).HasMaxLength(EntityConstraints.TransactionIdMaxLength);
            entityBuilder.HasOne(x => x.Product).WithMany(x => x.ProductStockRemovedEvents).HasForeignKey(x => x.ProductId);
        }

        private void ConfigureProductTakenOutEventsTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<ProductTakenOutEvent>();

            entityBuilder.ForSqlServerHasIndex(x => x.TransactionId).IsUnique(false);

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.TransactionId).IsRequired(true).HasMaxLength(EntityConstraints.TransactionIdMaxLength);
            entityBuilder.HasOne(x => x.Product).WithMany(x => x.ProductTakenOutEvents).HasForeignKey(x => x.ProductId);
        }

        private void ConfigureProductBroughtBackEventsTable(ModelBuilder modelBuilder)
        {
            var entityBuilder = modelBuilder.Entity<ProductBroughtBackEvent>();

            entityBuilder.ForSqlServerHasIndex(x => x.TransactionId).IsUnique(false);

            entityBuilder.HasKey(x => x.Id).ForSqlServerIsClustered(true);
            entityBuilder.Property(x => x.Id).UseSqlServerIdentityColumn();
            entityBuilder.Property(x => x.TransactionId).IsRequired(true).HasMaxLength(EntityConstraints.TransactionIdMaxLength);
            entityBuilder.HasOne(x => x.Product).WithMany(x => x.ProductBroughtBackEvents).HasForeignKey(x => x.ProductId);
        }
    }
}