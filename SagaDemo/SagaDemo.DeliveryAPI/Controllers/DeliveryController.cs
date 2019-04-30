using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.DeliveryAPI.Handlers.CommandHandlers;
using SagaDemo.DeliveryAPI.Mappers;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler;
        private readonly IRegisterDeliveryAttemptCommandHandler registerDeliveryAttemptCommandHandler;

        public DeliveryController(
            ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler,
            IRegisterDeliveryAttemptCommandHandler registerDeliveryAttemptCommandHandler)
        {
            this.createDeliveryRequestCommandHandler = createDeliveryRequestCommandHandler ?? throw new ArgumentNullException(nameof(createDeliveryRequestCommandHandler));
            this.registerDeliveryAttemptCommandHandler = registerDeliveryAttemptCommandHandler ?? throw new ArgumentNullException(nameof(registerDeliveryAttemptCommandHandler));
        }

        [HttpPost("{transactionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> CreateDeliveryRequest(string transactionId, Contracts.DataStructures.Address address, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(transactionId, address);

            await createDeliveryRequestCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/delivery-attempts")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RegisterDeliveryAttempt(string transactionId, CancellationToken cancellationToken)
        {
            var command = new RegisterDeliveryAttemptCommand(transactionId);

            await registerDeliveryAttemptCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}