namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class RegisterDeliveryAttemptCommand
    {
        public RegisterDeliveryAttemptCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}