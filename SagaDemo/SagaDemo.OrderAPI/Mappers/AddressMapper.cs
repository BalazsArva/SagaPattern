namespace SagaDemo.OrderAPI.Mappers
{
    public static class AddressMapper
    {
        public static DeliveryAPI.ApiClient.Address ToDeliveryApiContract(Operations.DataStructures.Address address)
        {
            if (address == null)
            {
                return null;
            }

            return new DeliveryAPI.ApiClient.Address(address.City, address.Country, address.House, address.State, address.Street, address.Zip);
        }
    }
}