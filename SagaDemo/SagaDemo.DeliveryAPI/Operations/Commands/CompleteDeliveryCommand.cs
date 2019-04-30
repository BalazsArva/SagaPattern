namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CompleteDeliveryCommand : IDeliveryCommand
    {
        public CompleteDeliveryCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}