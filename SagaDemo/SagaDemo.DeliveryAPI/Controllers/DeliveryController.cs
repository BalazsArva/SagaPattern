using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.Common.AspNetCore;
using SagaDemo.DeliveryAPI.Handlers.CommandHandlers;
using SagaDemo.DeliveryAPI.Handlers.QueryHandlers;
using SagaDemo.DeliveryAPI.Mappers;
using SagaDemo.DeliveryAPI.Operations.Commands;
using SagaDemo.DeliveryAPI.Operations.Queries;
using SagaDemo.DeliveryAPI.Operations.Results;

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
        private readonly IGetDeliveryByIdQueryHandler getDeliveryByIdQueryHandler;

        public DeliveryController(
            ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler,
            IRegisterDeliveryAttemptCommandHandler registerDeliveryAttemptCommandHandler,
            ICompleteDeliveryCommandHandler completeDeliveryCommandHandler,
            ICancelDeliveryCommandHandler cancelDeliveryCommandHandler,
            IGetDeliveryByIdQueryHandler getDeliveryByIdQueryHandler)
        {
            this.createDeliveryRequestCommandHandler = createDeliveryRequestCommandHandler ?? throw new ArgumentNullException(nameof(createDeliveryRequestCommandHandler));
            this.registerDeliveryAttemptCommandHandler = registerDeliveryAttemptCommandHandler ?? throw new ArgumentNullException(nameof(registerDeliveryAttemptCommandHandler));
            this.completeDeliveryCommandHandler = completeDeliveryCommandHandler ?? throw new ArgumentNullException(nameof(completeDeliveryCommandHandler));
            this.cancelDeliveryCommandHandler = cancelDeliveryCommandHandler ?? throw new ArgumentNullException(nameof(cancelDeliveryCommandHandler));
            this.getDeliveryByIdQueryHandler = getDeliveryByIdQueryHandler ?? throw new ArgumentNullException(nameof(getDeliveryByIdQueryHandler));
        }

        [HttpGet("{transactionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDeliveryByIdQueryResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetDeliveryDetails(string transactionId, CancellationToken cancellationToken)
        {
            var query = new GetDeliveryByIdQuery(transactionId);

            var result = await getDeliveryByIdQueryHandler.HandleAsync(query, cancellationToken).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            Response.Headers.Add(CustomHttpHeaderKeys.EntityVersion, result.DocumentVersion);

            return Ok(result.Delivery);
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