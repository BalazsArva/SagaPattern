using System.Collections.Generic;

namespace SagaDemo.OrderAPI.Entitites
{
    public class OrderDetails
    {
        public int UserId { get; set; }

        public List<OrderItem> Items { get; set; }

        public Address Address { get; set; }
    }
}