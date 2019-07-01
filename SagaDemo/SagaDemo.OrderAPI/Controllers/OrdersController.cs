using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SagaDemo.OrderAPI.Contracts.Requests;
using SagaDemo.OrderAPI.Mappers;
using SagaDemo.OrderAPI.Services.Handlers.CommandHandlers;

namespace SagaDemo.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IRegisterOrderCommandHandler registerOrderCommandHandler;

        public OrdersController(IRegisterOrderCommandHandler registerOrderCommandHandler)
        {
            this.registerOrderCommandHandler = registerOrderCommandHandler ?? throw new ArgumentNullException(nameof(registerOrderCommandHandler));
        }

        [HttpPost]
        public async Task<IActionResult> Post(RegisterOrderRequest request, CancellationToken cancellationToken)
        {
            var command = RegisterOrderMapper.ToCommand(request);

            await registerOrderCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Return Created instead, include object with details
            return Ok();
        }
    }
}