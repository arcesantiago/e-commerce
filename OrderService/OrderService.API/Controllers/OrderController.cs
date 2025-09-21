using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Application.Features.Orders.Queries.GetOrders;
using System.Net;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>order</returns>
        [HttpGet("{id}", Name = "GetOrder")]
        [ProducesResponseType(typeof(OrderVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderVm>> GetOrder([FromRoute] int id)
        {
            return Ok(await _mediator.Send(new GetOrderQuery(id)));
        }

        /// <summary>
        /// get all orders
        /// </summary>
        /// <returns>list of orders</returns>
        [HttpGet(Name = "GetOrders")]
        [ProducesResponseType(typeof(List<OrdersVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<OrdersVm>>> GetOrders()
        {
            return Ok(await _mediator.Send(new GetOrdersQuery()));
        }

        /// <summary>
        /// create order
        /// </summary>
        /// <param name="request"></param>
        /// <returns>order id</returns>
        [HttpPost(Name = "CreateOrder")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderCommandRequest request)
        {
            return Ok(await _mediator.Send(new CreateOrderCommand(request)));
        }

        /// <summary>
        /// update order status
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut(Name = "UpdateOrderStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusCommand request)
        {
            await _mediator.Send(request);

            return NoContent();
        }
    }
}
