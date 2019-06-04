namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class BringbackItemRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}