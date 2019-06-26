namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemsCommand
    {
        public BringbackItemsCommand(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}