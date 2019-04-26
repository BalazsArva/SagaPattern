﻿using System;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<IActionResult> EarnPoints(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            await earnPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundPoints(RefundPointsCommand command, CancellationToken cancellationToken)
        {
            await refundPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("consume")]
        public async Task<IActionResult> ConsumePoints(ConsumePointsCommand command, CancellationToken cancellationToken)
        {
            await consumePointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}