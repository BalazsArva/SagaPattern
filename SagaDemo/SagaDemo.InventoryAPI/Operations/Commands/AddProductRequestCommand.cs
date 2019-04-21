namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddProductRequestCommand
    {
        public AddProductRequestCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}