using System;
using Microsoft.EntityFrameworkCore;

namespace SagaDemo.InventoryAPI.DataAccess
{
    public class InventoryDbContextFactory : IInventoryDbContextFactory
    {
        private readonly DbContextOptions<InventoryDbContext> dbContextOptions;

        public InventoryDbContextFactory(DbContextOptions<InventoryDbContext> dbContextOptions)
        {
            this.dbContextOptions = dbContextOptions ?? throw new ArgumentNullException(nameof(dbContextOptions));
        }

        public InventoryDbContext CreateDbContext()
        {
            return new InventoryDbContext(dbContextOptions);
        }
    }
}