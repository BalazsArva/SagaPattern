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

            // TODO: Configure entities
        }
    }
}