namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemCommand
    {
        public BringbackItemCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}