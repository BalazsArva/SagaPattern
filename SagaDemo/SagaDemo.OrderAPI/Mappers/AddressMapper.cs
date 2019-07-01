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

        public static Entitites.Address ToEntity(Operations.DataStructures.Address address)
        {
            if (address == null)
            {
                return null;
            }

            return new Entitites.Address
            {
                City = address.City,
                Country = address.Country,
                House = address.House,
                State = address.State,
                Street = address.Street,
                Zip = address.Zip
            };
        }

        public static Operations.DataStructures.Address ToOperationsDataStructure(Contracts.DataStructures.Address address)
        {
            if (address == null)
            {
                return null;
            }

            return new Operations.DataStructures.Address
            {
                City = address.City,
                Country = address.Country,
                House = address.House,
                State = address.State,
                Street = address.Street,
                Zip = address.Zip
            };
        }
    }
}