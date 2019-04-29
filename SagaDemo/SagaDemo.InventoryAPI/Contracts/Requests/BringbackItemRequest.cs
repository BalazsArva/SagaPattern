namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class BringbackItemRequest
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
    }
}