using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface ICancelReservationsCommandHandler
    {
        Task HandleAsync(CancelReservationsCommand command, CancellationToken cancellationToken);
    }
}