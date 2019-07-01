using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Operations.Commands
{
    public class RegisterOrderCommand
    {
        public int UserId { get; set; }

        public Address Address { get; set; }

        public Order Order { get; set; }
    }
}