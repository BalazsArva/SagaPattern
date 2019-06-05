using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemsCommand
    {
        public TakeoutItemsCommand(IEnumerable<TakeoutItemCommand> items, string transactionId)
        {
            Items = items?.ToList() ?? Enumerable.Empty<TakeoutItemCommand>();
            TransactionId = transactionId;
        }

        public IEnumerable<TakeoutItemCommand> Items { get; }

        public string TransactionId { get; }
    }
}