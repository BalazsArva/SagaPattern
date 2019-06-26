using System;

namespace SagaDemo.OrderAPI.Providers
{
    public class GuidProvider : IGuidProvider
    {
        public string GenerateGuidString() => Guid.NewGuid().ToString();
    }
}