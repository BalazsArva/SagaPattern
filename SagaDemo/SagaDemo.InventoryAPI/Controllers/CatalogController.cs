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
        private readonly IAddReservationsCommandHandler addProductReservationsCommandHandler;

        public CatalogController(
            ICreateProductCommandHandler createProductCommandHandler,
            IGetProductByIdRequestHandler getProductByIdRequestHandler,
            IAddReservationsCommandHandler addProductReservationsCommandHandler)
        {
            this.createProductCommandHandler = createProductCommandHandler ?? throw new ArgumentNullException(nameof(createProductCommandHandler));
            this.getProductByIdRequestHandler = getProductByIdRequestHandler ?? throw new ArgumentNullException(nameof(getProductByIdRequestHandler));
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
        public async Task<IActionResult> GetItem(int id, CancellationToken cancellationToken)
        {
            var request = new GetProductByIdRequest { ProductId = id };
            var response = await getProductByIdRequestHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost("{transactionId}/reserve")]
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