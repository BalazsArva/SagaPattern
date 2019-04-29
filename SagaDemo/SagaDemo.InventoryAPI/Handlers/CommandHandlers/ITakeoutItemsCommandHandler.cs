using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface ITakeoutItemsCommandHandler
    {
        Task HandleAsync(TakeoutItemsCommand command, CancellationToken cancellationToken);
    }
}