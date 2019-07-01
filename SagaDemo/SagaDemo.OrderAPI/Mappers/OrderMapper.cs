using System;
using System.Collections.Generic;
using System.Linq;
using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Mappers
{
    public static class OrderMapper
    {
        public static List<Entitites.OrderItem> ToEntities(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            return order.Items.Select(i => OrderItemMapper.ToEntity(i)).ToList();
        }

        public static Order ToOperationsDataStructure(Contracts.DataStructures.Order order)
        {
            if (order == null)
            {
                return null;
            }

            return new Order
            {
                Items = order.Items.Select(i => OrderItemMapper.ToOperationsDataStructure(i)).ToList()
            };
        }
    }
}