namespace SagaDemo.LoyaltyPointsAPI.Operations.Commands
{
    public class ConsumePointsCommand
    {
        public ConsumePointsCommand(int points, int userId, string transactionId)
        {
            Points = points;
            UserId = userId;
            TransactionId = transactionId;
        }

        public int Points { get; }

        public int UserId { get; }

        public string TransactionId { get; }
    }
}