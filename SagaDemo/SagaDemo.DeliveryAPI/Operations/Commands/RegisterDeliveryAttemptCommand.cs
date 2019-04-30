namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class RegisterDeliveryAttemptCommand : IDeliveryCommand
    {
        public RegisterDeliveryAttemptCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}