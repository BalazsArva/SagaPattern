using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class RemoveStocksRequest
    {
        public List<RemoveStockRequest> Items { get; set; }
    }
}