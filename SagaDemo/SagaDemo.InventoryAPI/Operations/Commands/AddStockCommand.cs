namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStockCommand
    {
        public AddStockCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}