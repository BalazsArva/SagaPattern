namespace SagaDemo.InventoryAPI.Operations.Requests
{
    public class GetProductByIdRequest
    {
        public GetProductByIdRequest(string productId)
        {
            ProductId = productId;
        }

        public string ProductId { get; }
    }
}