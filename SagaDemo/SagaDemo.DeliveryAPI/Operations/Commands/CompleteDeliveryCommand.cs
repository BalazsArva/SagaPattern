namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CompleteDeliveryCommand
    {
        public CompleteDeliveryCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}