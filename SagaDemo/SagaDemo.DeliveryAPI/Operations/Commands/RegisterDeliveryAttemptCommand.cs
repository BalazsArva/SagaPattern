namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class RegisterDeliveryAttemptCommand : IDeliveryCommand
    {
        public RegisterDeliveryAttemptCommand(string transactionId, string documentVersion)
        {
            TransactionId = transactionId;
            DocumentVersion = documentVersion;
        }

        public string TransactionId { get; }

        public string DocumentVersion { get; }
    }
}