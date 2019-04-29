using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Contracts.Requests;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;
using SagaDemo.InventoryAPI.Mappers;
using SagaDemo.InventoryAPI.Operations.Requests;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICreateProductCommandHandler createProductCommandHandler;
        private readonly IGetProductByIdRequestHandler getProductByIdRequestHandler;
        private readonly IAddStocksCommandHandler addStocksCommandHandler;
        private readonly ITakeoutItemsCommandHandler takeoutItemsCommandHandler;
        private readonly IBringbackItemsCommandHandler bringbackItemsCommandHandler;
        private readonly IAddReservationsCommandHandler addProductReservationsCommandHandler;

        public CatalogController(
            ICreateProductCommandHandler createProductCommandHandler,
            IGetProductByIdRequestHandler getProductByIdRequestHandler,
            IAddStocksCommandHandler addStocksCommandHandler,
            ITakeoutItemsCommandHandler takeoutItemsCommandHandler,
            IBringbackItemsCommandHandler bringbackItemsCommandHandler,
            IAddReservationsCommandHandler addProductReservationsCommandHandler)
        {
            this.createProductCommandHandler = createProductCommandHandler ?? throw new ArgumentNullException(nameof(createProductCommandHandler));
            this.getProductByIdRequestHandler = getProductByIdRequestHandler ?? throw new ArgumentNullException(nameof(getProductByIdRequestHandler));
            this.addStocksCommandHandler = addStocksCommandHandler ?? throw new ArgumentNullException(nameof(addStocksCommandHandler));
            this.takeoutItemsCommandHandler = takeoutItemsCommandHandler ?? throw new ArgumentNullException(nameof(takeoutItemsCommandHandler));
            this.bringbackItemsCommandHandler = bringbackItemsCommandHandler ?? throw new ArgumentNullException(nameof(bringbackItemsCommandHandler));
            this.addProductReservationsCommandHandler = addProductReservationsCommandHandler ?? throw new ArgumentNullException(nameof(addProductReservationsCommandHandler));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> CreateItem(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            var response = await createProductCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return CreatedAtRoute(RouteNames.GetCatalogItem, new { id = response.ProductId }, response);
        }

        [HttpGet("{id}", Name = RouteNames.GetCatalogItem)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductByIdResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(void))]
        public async Task<IActionResult> GetItem(string id, CancellationToken cancellationToken)
        {
            var response = await getProductByIdRequestHandler.HandleAsync(new GetProductByIdRequest(id), cancellationToken).ConfigureAwait(false);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost("reservations")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> ReserveItems(AddReservationsRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await addProductReservationsCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return NoContent();
        }

        // TODO: Create separate controller for stock management.
        [HttpPost("add-to-stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> AddStocks(AddStocksRequest request, CancellationToken cancellationToken)
        {
            var command = ApiContractMapper.ToServiceCommand(request);

            await addStocksCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

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