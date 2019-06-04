namespace SagaDemo.InventoryAPI.DataAccess
{
    public interface IInventoryDbContextFactory
    {
        InventoryDbContext CreateDbContext();
    }
}