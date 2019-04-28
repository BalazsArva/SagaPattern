using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddProductReservationsCommand
    {
        public AddProductReservationsCommand(IEnumerable<AddProductReservationCommand> subcommands)
        {
            Reservations = subcommands?.ToList() ?? throw new ArgumentNullException(nameof(subcommands));
        }

        public IEnumerable<AddProductReservationCommand> Reservations { get; }
    }
}