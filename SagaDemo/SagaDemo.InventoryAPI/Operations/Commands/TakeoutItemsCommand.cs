using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class TakeoutItemsCommand
    {
        public TakeoutItemsCommand(IEnumerable<TakeoutItemCommand> subcommands)
        {
            Takeouts = subcommands?.ToList() ?? throw new ArgumentNullException(nameof(subcommands));
        }

        public IEnumerable<TakeoutItemCommand> Takeouts { get; }
    }
}