using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationsCommand
    {
        public AddReservationsCommand(IEnumerable<AddReservationCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddReservationCommand>();
        }

        public IEnumerable<AddReservationCommand> Items { get; }
    }
}