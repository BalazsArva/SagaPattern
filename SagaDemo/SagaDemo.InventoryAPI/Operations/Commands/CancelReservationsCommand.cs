namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class CancelReservationsCommand
    {
        public CancelReservationsCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}