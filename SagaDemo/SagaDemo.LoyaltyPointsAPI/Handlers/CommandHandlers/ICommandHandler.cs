using System.Threading;
using System.Threading.Tasks;

namespace SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers
{
    public interface ICommandHandler<TCommand>
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
