using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStocksCommand
    {
        public AddStocksCommand(IEnumerable<AddStockCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddStockCommand>();
        }

        public IEnumerable<AddStockCommand> Items { get; }
    }
}