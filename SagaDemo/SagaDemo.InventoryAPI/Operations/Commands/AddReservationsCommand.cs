using System.Collections.Generic;
using System.Linq;

namespace SagaDemo.InventoryAPI.Operations.Commands
{
    public class AddReservationsCommand
    {
        public AddReservationsCommand(IEnumerable<AddReservationCommand> items, string transactionId)
        {
            Items = items?.ToList() ?? Enumerable.Empty<AddReservationCommand>();
            TransactionId = transactionId;
        }

        public IEnumerable<AddReservationCommand> Items { get; }

        public string TransactionId { get; }
    }
}