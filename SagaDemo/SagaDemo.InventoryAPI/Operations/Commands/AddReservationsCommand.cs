using System;
using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationsCommand
    {
        public AddReservationsCommand(IEnumerable<AddReservationCommand> subcommands)
        {
            Reservations = subcommands?.ToList() ?? throw new ArgumentNullException(nameof(subcommands));
        }

        public IEnumerable<AddReservationCommand> Reservations { get; }
    }
}