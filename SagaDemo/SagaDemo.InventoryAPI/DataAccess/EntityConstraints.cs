namespace SagaDemo.InventoryAPI.DataAccess
{
    public static class EntityConstraints
    {
        public const int TransactionIdMaxLength = 256;

        public static class Product
        {
            public const int NameMaxLength = 256;
        }
    }
}