using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microservices.Sample.Order.Application.Commands;
using Microservices.Sample.Order.Application.Queries;

namespace Microservices.Sample.Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery(id));
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var orderId = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, orderId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", command.CustomerId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Order ID mismatch");
            }

            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
