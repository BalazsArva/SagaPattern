using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemCommand
    {
        [JsonConstructor]
        public BringbackItemCommand(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}