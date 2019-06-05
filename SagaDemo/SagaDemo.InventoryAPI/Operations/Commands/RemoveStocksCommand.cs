using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class RemoveStocksCommand
    {
        public RemoveStocksCommand(IEnumerable<RemoveStockCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<RemoveStockCommand>();
        }

        public IEnumerable<RemoveStockCommand> Items { get; }
    }
}