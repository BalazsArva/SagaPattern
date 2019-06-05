namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationCommand
    {
        public AddReservationCommand(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}