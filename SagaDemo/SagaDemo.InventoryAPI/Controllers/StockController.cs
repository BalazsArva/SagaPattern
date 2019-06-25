﻿using System;
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
    public class StockController : ControllerBase
    {
        private readonly IAddStocksCommandHandler addStocksCommandHandler;
        private readonly IRemoveStocksCommandHandler removeStocksCommandHandler;
        private readonly ITakeoutItemsCommandHandler takeoutItemsCommandHandler;
        private readonly IBringbackItemsCommandHandler bringbackItemsCommandHandler;

        public StockController(
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

        [HttpPost("stocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> AddStocks(AddStocksRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await addStocksCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpDelete("stocks")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> RemoveStocks(RemoveStocksRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await removeStocksCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("takeout")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> TakeoutItem(TakeoutItemsRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await takeoutItemsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        [HttpPost("bringback")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> BringbackItem(BringbackItemsRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await bringbackItemsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }
    }
}