namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemCommand
    {
        public TakeoutItemCommand(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}