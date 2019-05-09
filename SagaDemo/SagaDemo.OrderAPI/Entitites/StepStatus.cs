namespace SagaDemo.OrderAPI.Entitites
{
    public enum StepStatus
    {
        NotStarted,

        Completed,

        RolledBack,

        TemporarFailure,

        PermanentFailure
    }
}