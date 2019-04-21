using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface IAddStocksCommandHandler
    {
        Task HandleAsync(AddStocksCommand command, CancellationToken cancellationToken);
    }
}