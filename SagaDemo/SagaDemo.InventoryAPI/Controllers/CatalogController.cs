using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Handlers.RequestHandlers;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Operations.Requests;

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
        public async Task<IActionResult> CreateItem(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var productId = await createProductCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
            var response = await getProductByIdRequestHandler.HandleAsync(new GetProductByIdRequest(productId), cancellationToken).ConfigureAwait(false);

            return CreatedAtAction(RouteNames.GetCatalogItem, new { id = productId }, response);
        }

        [HttpGet("{id}", Name = RouteNames.GetCatalogItem)]
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