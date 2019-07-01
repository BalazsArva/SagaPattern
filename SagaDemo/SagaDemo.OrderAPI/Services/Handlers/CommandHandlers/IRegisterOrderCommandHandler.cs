using System.Threading;
using System.Threading.Tasks;
using SagaDemo.OrderAPI.Operations.Commands;

namespace SagaDemo.OrderAPI.Services.Handlers.CommandHandlers
{
    public interface IRegisterOrderCommandHandler
    {
        Task<string> HandleAsync(RegisterOrderCommand command, CancellationToken cancellationToken);
    }
}