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

        public static Contracts.DataStructures.DeliveryStatus ToApiContract(Operations.DataStructures.DeliveryStatus status)
        {
            switch (status)
            {
                case Operations.DataStructures.DeliveryStatus.Cancelled:
                    return Contracts.DataStructures.DeliveryStatus.Cancelled;

                case Operations.DataStructures.DeliveryStatus.Created:
                    return Contracts.DataStructures.DeliveryStatus.Created;

                case Operations.DataStructures.DeliveryStatus.Finished:
                    return Contracts.DataStructures.DeliveryStatus.Finished;

                case Operations.DataStructures.DeliveryStatus.InProgress:
                    return Contracts.DataStructures.DeliveryStatus.InProgress;

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), $"The value of the {nameof(status)} is not among the acceptable values.");
            }
        }
    }
}