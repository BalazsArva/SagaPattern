using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SagaDemo.InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateItem(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
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