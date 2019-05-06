using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.Common.AspNetCore;
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
        private readonly ICompleteDeliveryCommandHandler completeDeliveryCommandHandler;
        private readonly ICancelDeliveryCommandHandler cancelDeliveryCommandHandler;

        public DeliveryController(
            ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler,
            IRegisterDeliveryAttemptCommandHandler registerDeliveryAttemptCommandHandler,
            ICompleteDeliveryCommandHandler completeDeliveryCommandHandler,
            ICancelDeliveryCommandHandler cancelDeliveryCommandHandler)
        {
            this.createDeliveryRequestCommandHandler = createDeliveryRequestCommandHandler ?? throw new ArgumentNullException(nameof(createDeliveryRequestCommandHandler));
            this.registerDeliveryAttemptCommandHandler = registerDeliveryAttemptCommandHandler ?? throw new ArgumentNullException(nameof(registerDeliveryAttemptCommandHandler));
            this.completeDeliveryCommandHandler = completeDeliveryCommandHandler ?? throw new ArgumentNullException(nameof(completeDeliveryCommandHandler));
            this.cancelDeliveryCommandHandler = cancelDeliveryCommandHandler ?? throw new ArgumentNullException(nameof(cancelDeliveryCommandHandler));
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
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(void))]
        public async Task<IActionResult> RegisterDeliveryAttempt(string transactionId, [FromHeader(Name = CustomHttpHeaderKeys.EntityVersion)]string documentVersion, CancellationToken cancellationToken)
        {
            var command = new RegisterDeliveryAttemptCommand(transactionId, documentVersion);

            await registerDeliveryAttemptCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(void))]
        public async Task<IActionResult> CompleteDelivery(string transactionId, [FromHeader(Name = CustomHttpHeaderKeys.EntityVersion)]string documentVersion, CancellationToken cancellationToken)
        {
            var command = new CompleteDeliveryCommand(transactionId, documentVersion);

            await completeDeliveryCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(void))]
        public async Task<IActionResult> CancelDelivery(string transactionId, [FromHeader(Name = CustomHttpHeaderKeys.EntityVersion)]string documentVersion, CancellationToken cancellationToken)
        {
            var command = new CancelDeliveryCommand(transactionId, documentVersion);

            await cancelDeliveryCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}