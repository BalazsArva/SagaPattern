using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStockCommand
    {
        [JsonConstructor]
        public AddStockCommand(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public string ProductId { get; }

        public int Quantity { get; }
    }
}