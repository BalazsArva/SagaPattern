using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationsCommand
    {
        [JsonConstructor]
        public AddReservationsCommand(IEnumerable<AddReservationCommand> items)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddReservationCommand>();
        }

        public IEnumerable<AddReservationCommand> Items { get; }
    }
}