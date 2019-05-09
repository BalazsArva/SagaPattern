using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Operations.Commands
{
    public class CreateOrderCommand
    {
        public CreateOrderCommand(int userId, Order order, Address address)
        {
            UserId = userId;
            Order = order;
            Address = address;
        }

        public int UserId { get; }

        public Address Address { get; }

        public Order Order { get; }
    }
}