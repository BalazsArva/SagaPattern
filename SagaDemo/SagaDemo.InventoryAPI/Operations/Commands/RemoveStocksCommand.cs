using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class RemoveStocksCommand
    {
        public RemoveStocksCommand(IEnumerable<RemoveStockCommand> items, string transactionId)
        {
            Items = items?.ToList() ?? Enumerable.Empty<RemoveStockCommand>();
            TransactionId = transactionId;
        }

        public IEnumerable<RemoveStockCommand> Items { get; }

        public string TransactionId { get; }
    }
}