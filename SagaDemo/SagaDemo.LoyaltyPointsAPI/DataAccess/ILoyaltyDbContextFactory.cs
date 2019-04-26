namespace SagaDemo.LoyaltyPointsAPI.DataAccess
{
    public interface ILoyaltyDbContextFactory
    {
        LoyaltyDbContext CreateDbContext();
    }
}