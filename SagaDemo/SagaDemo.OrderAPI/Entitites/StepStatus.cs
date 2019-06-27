namespace SagaDemo.OrderAPI.Entitites
{
    public enum StepStatus
    {
        NotStarted,

        InProgress,

        Completed,

        RolledBack,

        TemporalFailure,

        PermanentFailure,

        RollbackFailed
    }
}