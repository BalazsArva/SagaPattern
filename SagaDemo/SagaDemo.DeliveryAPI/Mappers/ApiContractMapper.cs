using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class ApiContractMapper
    {
        public static CreateDeliveryRequestCommand ToServiceCommand(string transactionId, Contracts.DataStructures.Address address)
        {
            return new CreateDeliveryRequestCommand(transactionId, ToServiceAddress(address));
        }

        public static Operations.DataStructures.Address ToServiceAddress(Contracts.DataStructures.Address address)
        {
            return new Operations.DataStructures.Address(address?.Country, address?.State, address?.City, address?.Zip, address?.Street, address?.House);
        }
    }
}