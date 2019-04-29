using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;
using SagaDemo.InventoryAPI.Operations.Commands;
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
        private readonly IAddProductReservationsCommandHandler addProductReservationsCommandHandler;

        public CatalogController(
            ICreateProductCommandHandler createProductCommandHandler,
            IGetProductByIdRequestHandler getProductByIdRequestHandler,
            IAddProductReservationsCommandHandler addProductReservationsCommandHandler)
        {
            this.createProductCommandHandler = createProductCommandHandler ?? throw new ArgumentNullException(nameof(createProductCommandHandler));
            this.getProductByIdRequestHandler = getProductByIdRequestHandler ?? throw new ArgumentNullException(nameof(getProductByIdRequestHandler));
            this.addProductReservationsCommandHandler = addProductReservationsCommandHandler ?? throw new ArgumentNullException(nameof(addProductReservationsCommandHandler));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateProductResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> CreateItem(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var response = await createProductCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            return CreatedAtAction(RouteNames.GetCatalogItem, new { id = response.ProductId }, response);
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

        [HttpPost("{id}/reservations")]
        public async Task<IActionResult> ReserveItem(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/takeout")]
        public async Task<IActionResult> TakeoutItem(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{id}/bringback")]
        public async Task<IActionResult> BringbackItem(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}