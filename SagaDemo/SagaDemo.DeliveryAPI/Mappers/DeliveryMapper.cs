using System;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;

namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class DeliveryMapper
    {
        public static Operations.DataStructures.Delivery ToServiceContract(IDocumentStore documentStore, Entities.Delivery deliveryEntity)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            if (deliveryEntity == null)
            {
                return null;
            }

            var id = DocumentIdHelper.GetEntityId<Entities.Delivery>(documentStore, deliveryEntity.Id);

            return new Operations.DataStructures.Delivery(
                id,
                AddressMapper.ToServiceContract(deliveryEntity.Address),
                DeliveryStatusMapper.ToServiceContract(deliveryEntity.Status));
        }

        public static Contracts.DataStructures.Delivery ToApiContract(Operations.DataStructures.Delivery delivery)
        {
            if (delivery == null)
            {
                return null;
            }

            return new Contracts.DataStructures.Delivery
            {
                Address = AddressMapper.ToApiContract(delivery.Address),
                Id = delivery.Id,
                Status = DeliveryStatusMapper.ToApiContract(delivery.Status)
            };
        }
    }
}