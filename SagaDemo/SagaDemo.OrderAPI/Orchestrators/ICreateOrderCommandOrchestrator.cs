using System.Threading;
using System.Threading.Tasks;
using SagaDemo.OrderAPI.Operations.Commands;

namespace SagaDemo.OrderAPI.Orchestrators
{
    public interface ICreateOrderCommandOrchestrator
    {
        Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken);
    }
}