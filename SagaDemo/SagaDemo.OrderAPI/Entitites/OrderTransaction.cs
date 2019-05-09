namespace SagaDemo.OrderAPI.Entitites
{
    public class OrderTransaction
    {
        public string Id { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public StepDetails LoyaltyPointsConsumptionStepDetails { get; set; }

        public StepDetails InventoryReservationStepDetails { get; set; }

        public StepDetails DeliveryCreationStepDetails { get; set; }
    }

    public enum StepStatus
    {
        NotStarted,

        Completed,

        RolledBack,

        TemporarFailure,

        PermanentFailure
    }

    public enum TransactionStatus
    {
        NotStarted,

        Completed,

        RolledBack,

        InProgress
    }

    public class StepDetails
    {
        public StepStatus StepStatus { get; set; }

        public int Attempts { get; set; }
    }
}