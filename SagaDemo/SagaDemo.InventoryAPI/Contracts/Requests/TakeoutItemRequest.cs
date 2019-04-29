namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class TakeoutItemRequest
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
    }
}