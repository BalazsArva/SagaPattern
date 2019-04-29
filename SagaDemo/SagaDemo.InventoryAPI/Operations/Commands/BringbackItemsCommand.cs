using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemsCommand
    {
        public BringbackItemsCommand(IEnumerable<BringbackItemCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<BringbackItemCommand>();
        }

        public IEnumerable<BringbackItemCommand> Items { get; }
    }
}