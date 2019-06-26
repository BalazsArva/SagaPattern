using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.LoyaltyPointsAPI.Contracts.Requests;
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

        [HttpPost("{transactionId}/earn")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> EarnPoints(string transactionId, EarnPointsRequest request, CancellationToken cancellationToken)
        {
            var command = new EarnPointsCommand(request.Points, request.UserId, transactionId);

            await earnPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/refund")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RefundPoints(string transactionId, CancellationToken cancellationToken)
        {
            var command = new RefundPointsCommand(transactionId);

            await refundPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/consume")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> ConsumePoints(string transactionId, ConsumePointsRequest request, CancellationToken cancellationToken)
        {
            var command = new ConsumePointsCommand(request.Points, request.UserId, transactionId);

            await consumePointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}