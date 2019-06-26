using System.Linq;
using SagaDemo.InventoryAPI.ApiClient;
using SagaDemo.OrderAPI.Operations.DataStructures;

namespace SagaDemo.OrderAPI.Mappers
{
    public static class ReservationsMapper
    {
        public static AddReservationsRequest ToReservationsApiContract(Order order)
        {
            if (order == null)
            {
                return null;
            }

            return new AddReservationsRequest(order.Items.Select(i => new AddReservationRequest(i.ProductId, i.Quantity)).ToList());
        }
    }
}