namespace SagaDemo.OrderAPI.Entitites
{
    public enum StepStatus
    {
        NotStarted,

        InProgress,

        Completed,

        RolledBack,

        TemporarFailure,

        PermanentFailure
    }
}