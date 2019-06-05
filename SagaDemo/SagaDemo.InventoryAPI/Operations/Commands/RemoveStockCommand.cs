namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class RemoveStockCommand
    {
        public RemoveStockCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}