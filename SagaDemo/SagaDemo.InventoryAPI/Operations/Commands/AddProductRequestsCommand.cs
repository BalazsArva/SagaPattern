using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddProductRequestsCommand
    {
        public AddProductRequestsCommand(IEnumerable<AddProductRequestCommand> subcommands)
        {
            Requests = subcommands?.ToList() ?? throw new ArgumentNullException(nameof(subcommands));
        }

        public IEnumerable<AddProductRequestCommand> Requests { get; }
    }
}