using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class BringbackItemsRequest
    {
        public List<BringbackItemRequest> Items { get; set; }

        public string TransactionId { get; set; }
    }
}