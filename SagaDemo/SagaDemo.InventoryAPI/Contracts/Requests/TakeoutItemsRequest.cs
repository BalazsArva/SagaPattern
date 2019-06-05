using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class TakeoutItemsRequest
    {
        public List<TakeoutItemRequest> Items { get; set; }

        public string TransactionId { get; set; }
    }
}