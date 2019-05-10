namespace SagaDemo.LoyaltyPointsAPI.Operations.Commands
{
    public class RefundPointsCommand
    {
        public RefundPointsCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}