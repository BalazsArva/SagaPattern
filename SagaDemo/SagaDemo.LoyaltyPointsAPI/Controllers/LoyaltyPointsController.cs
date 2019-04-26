using System;
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

        public LoyaltyPointsController(ICommandHandler<EarnPointsCommand> earnPointsCommandHandler)
        {
            this.earnPointsCommandHandler = earnPointsCommandHandler ?? throw new ArgumentNullException(nameof(earnPointsCommandHandler));
        }

        [HttpPost("earn")]
        public async Task<IActionResult> EarnPoints(EarnPointsCommand command, CancellationToken cancellationToken)
        {
            await earnPointsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}