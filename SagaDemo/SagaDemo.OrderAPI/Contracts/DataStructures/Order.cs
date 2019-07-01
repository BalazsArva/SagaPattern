using System.Collections.Generic;

namespace SagaDemo.OrderAPI.Contracts.DataStructures
{
    public class Order
    {
        public IEnumerable<OrderItem> Items { get; set; }
    }
}