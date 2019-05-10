namespace SagaDemo.LoyaltyPointsAPI.Validation
{
    public static class ValidationMessages
    {
        public const string ConsumedPointsExceedTotal = "Consumed points exceed available amount.";
        public const string PointsMustBePositive = "The points must be positive.";
        public const string PointsConsumptionNotFound = "No points consumptio transaction could be found with the specified transaction identifier.";
    }
}