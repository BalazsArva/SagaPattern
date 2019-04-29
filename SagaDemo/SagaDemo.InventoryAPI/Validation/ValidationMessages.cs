namespace SagaDemo.InventoryAPI.Validation
{
    public static class ValidationMessages
    {
        public const string QuantityMustBePositive = "The quantity must be positive.";
        public const string QuantityExceedsAvailable = "The quantity exceeds the available amount.";
        public const string ProductDoesNotExist = "This product does not exist.";
    }
}