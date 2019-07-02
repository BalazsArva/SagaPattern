using System;

namespace SagaDemo.OrderAPI.Entitites
{
    public class OrderTransaction
    {
        public string Id { get; set; }

        public DateTime? UtcDateTimeLockAcquired { get; set; }

        public DateTime? UtcDoNotExecuteBefore { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public StepDetails LoyaltyPointsConsumptionStepDetails { get; set; }

        public StepDetails InventoryReservationStepDetails { get; set; }

        public StepDetails DeliveryCreationStepDetails { get; set; }

        public OrderDetails OrderDetails { get; set; }
    }
}