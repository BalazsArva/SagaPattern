namespace SagaDemo.InventoryAPI.Entities
{
    public class Product
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int PointsCost { get; set; }

        public int StockCount { get; set; }

        public int RequestCount { get; set; }
    }
}