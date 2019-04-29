using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStocksCommand
    {
        [JsonConstructor]
        public AddStocksCommand(IEnumerable<AddStockCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddStockCommand>();
        }

        public IEnumerable<AddStockCommand> Items { get; }
    }
}