namespace SagaDemo.OrderAPI.Operations.DataStructures
{
    public class OrderItem
    {
        public OrderItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}