using SagaDemo.OrderAPI.Contracts.DataStructures;

namespace SagaDemo.OrderAPI.Contracts.Requests
{
    public class RegisterOrderRequest
    {
        public int UserId { get; set; }

        public Address Address { get; set; }

        public Order Order { get; set; }
    }
}