using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class CommandMapper
    {
        public static CreateDeliveryRequestCommand ToServiceCommand(string transactionId, Contracts.DataStructures.Address address)
        {
            return new CreateDeliveryRequestCommand(transactionId, AddressMapper.ToServiceContract(address));
        }
    }
}