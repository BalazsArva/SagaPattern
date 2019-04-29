using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemsCommand
    {
        [JsonConstructor]
        public TakeoutItemsCommand(IEnumerable<TakeoutItemCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<TakeoutItemCommand>();
        }

        public IEnumerable<TakeoutItemCommand> Items { get; }
    }
}