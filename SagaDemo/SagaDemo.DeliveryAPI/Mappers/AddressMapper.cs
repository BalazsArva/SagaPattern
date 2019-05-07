namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class AddressMapper
    {
        public static Operations.DataStructures.Address ToServiceContract(Entities.Address addressEntity)
        {
            if (addressEntity == null)
            {
                return null;
            }

            return new Operations.DataStructures.Address(addressEntity.Country, addressEntity.State, addressEntity.City, addressEntity.Zip, addressEntity.Street, addressEntity.House);
        }

        public static Operations.DataStructures.Address ToServiceContract(Contracts.DataStructures.Address address)
        {
            if (address == null)
            {
                return null;
            }

            return new Operations.DataStructures.Address(address.Country, address.State, address.City, address.Zip, address.Street, address.House);
        }

        public static Contracts.DataStructures.Address ToApiContract(Operations.DataStructures.Address address)
        {
            if (address == null)
            {
                return null;
            }

            return new Contracts.DataStructures.Address
            {
                Country = address.Country,
                State = address.State,
                City = address.City,
                Zip = address.Zip,
                Street = address.Street,
                House = address.House
            };
        }
    }
}