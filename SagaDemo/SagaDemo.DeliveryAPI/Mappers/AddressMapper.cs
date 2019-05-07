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
    }
}