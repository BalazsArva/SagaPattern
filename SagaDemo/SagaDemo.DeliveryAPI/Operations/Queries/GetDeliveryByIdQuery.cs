namespace SagaDemo.DeliveryAPI.Operations.Queries
{
    public class GetDeliveryByIdQuery
    {
        public GetDeliveryByIdQuery(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }
}