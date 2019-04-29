using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemsCommand
    {
        public TakeoutItemsCommand(IEnumerable<TakeoutItemCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<TakeoutItemCommand>();
        }

        public IEnumerable<TakeoutItemCommand> Items { get; }
    }
}