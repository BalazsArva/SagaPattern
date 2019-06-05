namespace SagaDemo.InventoryAPI.DataAccess.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int PointsCost { get; set; }

        public byte[] RowVersion { get; set; }
    }
}