namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class AddReservationRequest
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
    }
}