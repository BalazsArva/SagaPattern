using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.OrderAPI.Operations.DataStructures
{
    public class Order
    {
        public Order(IEnumerable<OrderItem> items)
        {
            Items = items.ToList();
        }

        public IEnumerable<OrderItem> Items { get; }
    }
}