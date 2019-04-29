using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class BringbackItemsCommand
    {
        [JsonConstructor]
        public BringbackItemsCommand(IEnumerable<BringbackItemCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<BringbackItemCommand>();
        }

        public IEnumerable<BringbackItemCommand> Items { get; }
    }
}