namespace SagaDemo.InventoryAPI.DataAccess.Entities
{
    public class ProductBroughtBackEvent
    {
        public int Id { get; set; }

        public string TransactionId { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}