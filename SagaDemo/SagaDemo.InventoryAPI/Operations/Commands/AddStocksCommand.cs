using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddStocksCommand
    {
        public AddStocksCommand(IEnumerable<AddStockCommand> subcommands)
        {
            Stocks = subcommands?.ToList() ?? throw new ArgumentNullException(nameof(subcommands));
        }

        public IEnumerable<AddStockCommand> Stocks { get; }
    }
}