using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Contracts.Requests;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Mappers;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IAddStocksCommandHandler addStocksCommandHandler;
        private readonly IRemoveStocksCommandHandler removeStocksCommandHandler;
        private readonly ITakeoutItemsCommandHandler takeoutItemsCommandHandler;
        private readonly IBringbackItemsCommandHandler bringbackItemsCommandHandler;

        public StocksController(
            IAddStocksCommandHandler addStocksCommandHandler,
            IRemoveStocksCommandHandler removeStocksCommandHandler,
            ITakeoutItemsCommandHandler takeoutItemsCommandHandler,
            IBringbackItemsCommandHandler bringbackItemsCommandHandler)
        {
            this.addStocksCommandHandler = addStocksCommandHandler ?? throw new ArgumentNullException(nameof(addStocksCommandHandler));
            this.removeStocksCommandHandler = removeStocksCommandHandler ?? throw new ArgumentNullException(nameof(removeStocksCommandHandler));
            this.takeoutItemsCommandHandler = takeoutItemsCommandHandler ?? throw new ArgumentNullException(nameof(takeoutItemsCommandHandler));
            this.bringbackItemsCommandHandler = bringbackItemsCommandHandler ?? throw new ArgumentNullException(nameof(bringbackItemsCommandHandler));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> AddStocks(AddStocksRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await addStocksCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RemoveStocks(RemoveStocksRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await removeStocksCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/takeout")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> TakeoutItem(string transactionId, TakeoutItemsRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(transactionId, request);

            await takeoutItemsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("{transactionId}/bringback")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> BringbackItem(string transactionId, CancellationToken cancellationToken)
        {
            var command = new BringbackItemsCommand(transactionId);

            await bringbackItemsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}