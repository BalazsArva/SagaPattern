using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface IAddProductRequestsCommandHandler
    {
        Task HandleAsync(AddProductRequestsCommand command, CancellationToken cancellationToken);
    }
}