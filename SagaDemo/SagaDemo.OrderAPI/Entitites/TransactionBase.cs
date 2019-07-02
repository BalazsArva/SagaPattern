using System;

namespace SagaDemo.OrderAPI.Entitites
{
    public abstract class TransactionBase
    {
        public string Id { get; set; }

        public DateTime? UtcDateTimeLockAcquired { get; set; }

        public DateTime? UtcDoNotExecuteBefore { get; set; }

        public TransactionStatus TransactionStatus { get; set; }
    }
}