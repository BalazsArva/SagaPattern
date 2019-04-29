using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface IBringbackItemsCommandHandler
    {
        Task HandleAsync(BringbackItemsCommand command, CancellationToken cancellationToken);
    }
}