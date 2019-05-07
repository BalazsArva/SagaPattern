namespace SagaDemo.DeliveryAPI.Operations.DataStructures
{
    public class Delivery
    {
        public Delivery(string id, Address address, DeliveryStatus status)
        {
            Id = id;
            Address = address;
            Status = status;
        }

        public string Id { get; }

        public Address Address { get; }

        public DeliveryStatus Status { get; }
    }
}