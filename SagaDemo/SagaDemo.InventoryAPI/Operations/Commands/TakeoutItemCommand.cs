namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemCommand
    {
        public TakeoutItemCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}