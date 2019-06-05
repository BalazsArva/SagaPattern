namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class TakeoutItemRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}