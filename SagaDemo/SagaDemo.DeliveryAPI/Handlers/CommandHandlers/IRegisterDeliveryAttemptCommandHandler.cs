using System.Threading;
using System.Threading.Tasks;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public interface IRegisterDeliveryAttemptCommandHandler
    {
        Task HandleAsync(RegisterDeliveryAttemptCommand command, CancellationToken cancellationToken);
    }
}