using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Contracts.Requests;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Mappers;

namespace SagaDemo.InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IAddReservationsCommandHandler addProductReservationsCommandHandler;

        public ReservationsController(IAddReservationsCommandHandler addProductReservationsCommandHandler)
        {
            this.addProductReservationsCommandHandler = addProductReservationsCommandHandler ?? throw new ArgumentNullException(nameof(addProductReservationsCommandHandler));
        }

        [HttpPost("{transactionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> ReserveItems(string transactionId, AddReservationsRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(transactionId, request);

            await addProductReservationsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}