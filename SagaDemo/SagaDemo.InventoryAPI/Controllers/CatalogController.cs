using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.InventoryAPI.Handlers.CommandHandlers;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICreateProductCommandHandler createProductCommandHandler;

        public CatalogController(ICreateProductCommandHandler createProductCommandHandler)
        {
            this.createProductCommandHandler = createProductCommandHandler ?? throw new ArgumentNullException(nameof(createProductCommandHandler));
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var productId = await createProductCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Retrieve created item and return as response
            // TODO: Fix ids, currently they will be like "products/1-A, we need int instead.
            return CreatedAtAction(RouteNames.GetCatalogItem, new { id = productId }, null);
        }

        [HttpGet("{id}", Name = RouteNames.GetCatalogItem)]
        public async Task<IActionResult> GetItem(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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