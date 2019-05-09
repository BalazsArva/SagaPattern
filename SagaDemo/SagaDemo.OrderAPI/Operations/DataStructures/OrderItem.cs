namespace SagaDemo.OrderAPI.Operations.DataStructures
{
    public class OrderItem
    {
        public OrderItem(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}