namespace SagaDemo.InventoryAPI.Operations.Responses
{
    public class GetProductByIdResponse
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int PointsCost { get; set; }
    }
}