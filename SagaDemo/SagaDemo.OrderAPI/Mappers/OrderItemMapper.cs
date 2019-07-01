using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Mappers
{
    public static class OrderItemMapper
    {
        public static Entitites.OrderItem ToEntity(OrderItem orderItem)
        {
            if (orderItem == null)
            {
                return null;
            }

            return new Entitites.OrderItem
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
        }

        public static OrderItem ToOperationsDataStructure(Contracts.DataStructures.OrderItem orderItem)
        {
            if (orderItem == null)
            {
                return null;
            }

            return new OrderItem
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
        }
    }
}