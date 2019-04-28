using System.Threading;
using System.Threading.Tasks;
using SagaDemo.InventoryAPI.Operations.Requests;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.RequestHandlers
{
    public interface IGetProductByIdRequestHandler
    {
        Task<GetProductByIdResponse> HandleAsync(GetProductByIdRequest request, CancellationToken cancellationToken);
    }
}