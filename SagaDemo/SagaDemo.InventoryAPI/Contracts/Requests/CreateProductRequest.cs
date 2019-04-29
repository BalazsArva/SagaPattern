namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class CreateProductRequest
    {
        public string Name { get; set; }

        public int PointsCost { get; set; }
    }
}