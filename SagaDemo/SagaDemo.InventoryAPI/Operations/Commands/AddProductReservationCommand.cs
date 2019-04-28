namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddProductReservationCommand
    {
        public AddProductReservationCommand(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}