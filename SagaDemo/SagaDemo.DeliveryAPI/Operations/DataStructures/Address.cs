namespace SagaDemo.DeliveryAPI.Operations.DataStructures
{
    public class Address
    {
        public Address(string country, string state, string city, string zip, string street, string house)
        {
            Country = country;
            State = state;
            City = city;
            Zip = zip;
            Street = street;
            House = house;
        }

        public string Country { get; }

        public string State { get; }

        public string City { get; }

        public string Zip { get; }

        public string Street { get; }

        public string House { get; }
    }
}