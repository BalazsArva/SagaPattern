using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class AddReservationsRequest
    {
        public List<AddReservationRequest> Items { get; set; }

        public string TransactionId { get; set; }
    }
}