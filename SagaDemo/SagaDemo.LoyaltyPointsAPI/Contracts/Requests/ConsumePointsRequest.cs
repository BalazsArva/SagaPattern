namespace SagaDemo.LoyaltyPointsAPI.Contracts.Requests
{
    public class ConsumePointsRequest
    {
        public int Points { get; set; }

        public int UserId { get; set; }
    }
}