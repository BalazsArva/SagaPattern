namespace SagaDemo.OrderAPI.Entitites
{
    public class OrderTransaction : TransactionBase
    {
        public StepDetails LoyaltyPointsConsumptionStepDetails { get; set; }

        public StepDetails InventoryReservationStepDetails { get; set; }

        public StepDetails DeliveryCreationStepDetails { get; set; }

        public OrderDetails OrderDetails { get; set; }
    }
}