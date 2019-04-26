namespace SagaDemo.LoyaltyPointsAPI.Validation
{
    public static class ValidationMessages
    {
        public const string CannotBeNullOrEmpty = "This value must be provided.";

        public const string ConsumedPointsExceedTotal = "Consumed points exceed available amount.";
    }
}