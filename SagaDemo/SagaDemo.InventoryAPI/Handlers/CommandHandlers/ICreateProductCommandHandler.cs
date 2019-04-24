using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface ICreateProductCommandHandler
    {
        Task<int> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken);
    }
}