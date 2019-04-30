using SagaDemo.DeliveryAPI.Operations.DataStructures;

namespace SagaDemo.DeliveryAPI.Operations.Commands
{
    public class CreateDeliveryRequestCommand
    {
        public CreateDeliveryRequestCommand(string transactionId, Address address)
        {
            TransactionId = transactionId;
            Address = address;
        }

        public string TransactionId { get; }

        public Address Address { get; }
    }
}