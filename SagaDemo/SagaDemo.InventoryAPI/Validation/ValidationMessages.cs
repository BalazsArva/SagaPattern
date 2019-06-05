namespace SagaDemo.InventoryAPI.Validation
{
    public static class ValidationMessages
    {
        public const string QuantityMustBePositive = "The quantity must be positive.";
        public const string QuantityExceedsAvailable = "The quantity exceeds the available amount.";
        public const string QuantityExceedsReservations = "The quantity exceeds the reserved amount.";
        public const string ProductDoesNotExist = "This product does not exist.";
        public const string TransactionIdRequired = "The transaction Id must be provided.";
    }
}