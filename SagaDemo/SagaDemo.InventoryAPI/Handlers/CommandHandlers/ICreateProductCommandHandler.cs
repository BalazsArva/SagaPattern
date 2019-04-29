using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public interface ICreateProductCommandHandler
    {
        Task<CreateProductResponse> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken);
    }
}