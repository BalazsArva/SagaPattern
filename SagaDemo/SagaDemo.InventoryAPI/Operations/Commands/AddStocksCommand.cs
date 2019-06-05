using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStocksCommand
    {
        public AddStocksCommand(IEnumerable<AddStockCommand> items, string transactionId)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddStockCommand>();
            TransactionId = transactionId;
        }

        public IEnumerable<AddStockCommand> Items { get; }

        public string TransactionId { get; }
    }
}