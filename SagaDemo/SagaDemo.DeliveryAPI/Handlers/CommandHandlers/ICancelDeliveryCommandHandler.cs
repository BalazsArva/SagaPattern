using System.Threading;
using System.Threading.Tasks;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public interface ICancelDeliveryCommandHandler
    {
        Task HandleAsync(CancelDeliveryCommand command, CancellationToken cancellationToken);
    }
}