using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.DeliveryAPI.Handlers.CommandHandlers;
using SagaDemo.DeliveryAPI.Mappers;

namespace SagaDemo.DeliveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler;

        public DeliveryController(ICreateDeliveryRequestCommandHandler createDeliveryRequestCommandHandler)
        {
            this.createDeliveryRequestCommandHandler = createDeliveryRequestCommandHandler ?? throw new ArgumentNullException(nameof(createDeliveryRequestCommandHandler));
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
    }
}