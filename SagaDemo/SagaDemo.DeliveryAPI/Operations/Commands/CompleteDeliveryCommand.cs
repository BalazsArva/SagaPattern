namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CompleteDeliveryCommand : IDeliveryCommand
    {
        public CompleteDeliveryCommand(string transactionId, string documentVersion)
        {
            TransactionId = transactionId;
            DocumentVersion = documentVersion;
        }

        public string TransactionId { get; }

        public string DocumentVersion { get; }
    }
}