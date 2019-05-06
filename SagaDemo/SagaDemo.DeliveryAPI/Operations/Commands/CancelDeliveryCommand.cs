namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CancelDeliveryCommand : IDeliveryCommand
    {
        public CancelDeliveryCommand(string transactionId, string documentVersion)
        {
            TransactionId = transactionId;
            DocumentVersion = documentVersion;
        }

        public string TransactionId { get; }

        public string DocumentVersion { get; }
    }
}