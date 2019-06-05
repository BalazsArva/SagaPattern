namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class RemoveStockRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}