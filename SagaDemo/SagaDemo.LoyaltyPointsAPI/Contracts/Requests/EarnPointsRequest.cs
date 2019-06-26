namespace SagaDemo.LoyaltyPointsAPI.Contracts.Requests
{
    public class EarnPointsRequest
    {
        public int Points { get; set; }

        public int UserId { get; set; }
    }
}