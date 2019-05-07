using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SagaDemo.DeliveryAPI.Contracts.DataStructures
{
    public class Delivery
    {
        public string Id { get; set; }

        public Address Address { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeliveryStatus Status { get; set; }
    }
}