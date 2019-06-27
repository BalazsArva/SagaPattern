namespace SagaDemo.OrderAPI.Entitites
{
    public enum TransactionStatus
    {
        NotStarted,

        InProgress,

        Completed,

        PermanentFailure,

        RolledBack,

        RollbackFailed
    }
}