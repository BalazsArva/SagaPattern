using System.Threading;
using System.Threading.Tasks;
using SagaDemo.DeliveryAPI.Operations.Queries;
using SagaDemo.DeliveryAPI.Operations.Results;

namespace SagaDemo.DeliveryAPI.Handlers.QueryHandlers
{
    public interface IGetDeliveryByIdQueryHandler
    {
        Task<GetDeliveryByIdQueryResult> HandleAsync(GetDeliveryByIdQuery query, CancellationToken cancellationToken);
    }
}