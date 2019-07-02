using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Operations.Commands
{
    public class CreateOrderCommand
    {
        public string TransactionId { get; set; }

        public int UserId { get; set; }

        public Address Address { get; set; }

        public Order Order { get; set; }
    }
}