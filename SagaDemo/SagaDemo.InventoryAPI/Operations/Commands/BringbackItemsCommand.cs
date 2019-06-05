using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemsCommand
    {
        public BringbackItemsCommand(IEnumerable<BringbackItemCommand> items, string transactionId)
        {
            Items = items?.ToList() ?? Enumerable.Empty<BringbackItemCommand>();
            TransactionId = transactionId;
        }

        public IEnumerable<BringbackItemCommand> Items { get; }

        public string TransactionId { get; }
    }
}