using System;

namespace SagaDemo.DeliveryAPI.Mappers
{
    public static class DeliveryStatusMapper
    {
        public static Operations.DataStructures.DeliveryStatus ToServiceContract(Entities.DeliveryStatus status)
        {
            switch (status)
            {
                case Entities.DeliveryStatus.Cancelled:
                    return Operations.DataStructures.DeliveryStatus.Cancelled;

                case Entities.DeliveryStatus.Created:
                    return Operations.DataStructures.DeliveryStatus.Created;

                case Entities.DeliveryStatus.Finished:
                    return Operations.DataStructures.DeliveryStatus.Finished;

                case Entities.DeliveryStatus.InProgress:
                    return Operations.DataStructures.DeliveryStatus.InProgress;

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), $"The value of the {nameof(status)} is not among the acceptable values.");
            }
        }
    }
}