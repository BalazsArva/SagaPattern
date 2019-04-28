namespace SagaDemo.InventoryAPI.Operations.Responses
{
    public class GetProductByIdResponse
    {
        public GetProductByIdResponse(string productId, string name, int pointsCost, int stockCount)
        {
            ProductId = productId;
            Name = name;
            PointsCost = pointsCost;
            StockCount = stockCount;
        }

        public string ProductId { get; }

        public string Name { get; }

        public int PointsCost { get; }

        public int StockCount { get; }
    }
}