using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.LoyaltyPointsAPI.Handlers.CommandHandlers;
using SagaDemo.LoyaltyPointsAPI.Operations.Commands;

namespace SagaDemo.LoyaltyPointsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyPointsController : ControllerBase
    {
        private readonly ICommandHandler<EarnPointsCommand> earnPointsCommandHandler;
        private readonly ICommandHandler<RefundPointsCommand> refundPointsCommandHandler;
        private readonly ICommandHandler<ConsumePointsCommand> consumePointsCommandHandler;

        public LoyaltyPointsController(
            ICommandHandler<EarnPointsCommand> earnPointsCommandHandler,
            ICommandHandler<RefundPointsCommand> refundPointsCommandHandler,
            ICommandHandler<ConsumePointsCommand> consumePointsCommandHandler)
        {
            this.earnPointsCommandHandler = earnPointsCommandHandler ?? throw new ArgumentNullException(nameof(earnPointsCommandHandler));
            this.refundPointsCommandHandler = refundPointsCommandHandler ?? throw new ArgumentNullException(nameof(refundPointsCommandHandler));
            this.consumePointsCommandHandler = consumePointsCommandHandler ?? throw new ArgumentNullException(nameof(consumePointsCommandHandler));
        }

        [HttpPost("earn")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> EarnPoints(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            await earnPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("refund")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RefundPoints(RefundPointsCommand command, CancellationToken cancellationToken)
        {
            // TODO: Consider returning 404 instead of 400 when the consume event does not exist.
            await refundPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("consume")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> ConsumePoints(ConsumePointsCommand command, CancellationToken cancellationToken)
        {
            await consumePointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}