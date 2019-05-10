using System;

namespace SagaDemo.LoyaltyPointsAPI.DataAccess.Entities
{
    public class PointsRefundedEvent
    {
        public int Id { get; protected set; }

        public int UserId { get; set; }

        public int PointChange { get; set; }

        public DateTime UtcDateTimeRecorded { get; set; }

        public string Reason { get; set; }

        public string TransactionId { get; set; }
    }
}