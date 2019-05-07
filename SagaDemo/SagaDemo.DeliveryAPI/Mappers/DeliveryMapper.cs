namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class DeliveryMapper
    {
        public static Operations.DataStructures.Delivery ToServiceContract(Entities.Delivery deliveryEntity)
        {
            if (deliveryEntity == null)
            {
                return null;
            }

            return new Operations.DataStructures.Delivery(
                deliveryEntity.Id,
                AddressMapper.ToServiceContract(deliveryEntity.Address),
                DeliveryStatusMapper.ToServiceContract(deliveryEntity.Status));
        }
    }
}