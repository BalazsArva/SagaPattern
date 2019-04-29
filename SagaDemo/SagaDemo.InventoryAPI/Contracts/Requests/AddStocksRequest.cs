using System.Collections.Generic;

namespace SagaDemo.InventoryAPI.Contracts.Requests
{
    public class AddStocksRequest
    {
        public List<AddStockRequest> Items { get; set; }
    }
}