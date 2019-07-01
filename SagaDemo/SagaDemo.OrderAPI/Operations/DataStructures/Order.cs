using System.Collections.Generic;

namespace SagaDemo.OrderAPI.Operations.DataStructures
{
    public class Order
    {
        public IEnumerable<OrderItem> Items { get; set; }
    }
}