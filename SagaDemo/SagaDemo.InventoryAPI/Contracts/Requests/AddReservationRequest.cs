namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class AddReservationRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}