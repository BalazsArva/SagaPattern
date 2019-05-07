using SagaDemo.DeliveryAPI.Operations.DataStructures;

namespace SagaDemo.DeliveryAPI.Operations.Results
{
    public class GetDeliveryByIdQueryResult
    {
        public GetDeliveryByIdQueryResult(Delivery delivery, string documentVersion)
        {
            Delivery = delivery;
            DocumentVersion = documentVersion;
        }

        public Delivery Delivery { get; }

        public string DocumentVersion { get; }
    }
}