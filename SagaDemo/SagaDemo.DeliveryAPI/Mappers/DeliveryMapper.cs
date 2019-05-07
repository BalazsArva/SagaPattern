using System;
using Raven.Client.Documents.Session;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;

namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class DeliveryMapper
    {
        public static Operations.DataStructures.Delivery ToServiceContract(IAsyncDocumentSession session, Entities.Delivery deliveryEntity)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (deliveryEntity == null)
            {
                return null;
            }

            var id = DocumentIdHelper.GetEntityId<Entities.Delivery>(session, deliveryEntity.Id);

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