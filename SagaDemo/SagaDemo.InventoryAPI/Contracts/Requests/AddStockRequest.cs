namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class AddStockRequest
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
    }
}