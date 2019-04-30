using System.Threading;
using System.Threading.Tasks;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public interface ICreateDeliveryRequestCommandHandler
    {
        Task HandleAsync(CreateDeliveryRequestCommand command, CancellationToken cancellationToken);
    }
}