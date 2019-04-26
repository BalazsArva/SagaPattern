namespace SagaDemo.LoyaltyPointsAPI.Operations.Commands
{
    public class RefundPointsCommand
    {
        public RefundPointsCommand(int points, int userId)
        {
            Points = points;
            UserId = userId;
        }

        public int Points { get; }

        public int UserId { get; }
    }
}