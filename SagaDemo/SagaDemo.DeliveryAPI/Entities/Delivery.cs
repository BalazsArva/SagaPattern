namespace SagaDemo.DeliveryAPI.Entities
{
    public class Delivery
    {
        public string Id { get; set; }

        public Address Address { get; set; }

        public DeliveryStatus Status { get; set; }
    }
}