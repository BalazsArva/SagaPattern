namespace SagaDemo.OrderAPI.Entitites
{
    public class StepDetails
    {
        public StepStatus StepStatus { get; set; }

        public int Attempts { get; set; }

        public int RollbackAttempts { get; set; }
    }
}