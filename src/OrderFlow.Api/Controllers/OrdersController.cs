using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Features.Orders.CreateOrder;
using OrderFlow.Application.Features.Orders.GetOrderById;
using OrderFlow.Application.Features.Orders.ListOrders;

namespace OrderFlow.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{

    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderCommand command)
    {

        var id = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetOrderById), new { id }, id);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailsDto>> GetOrderById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));


        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpGet]
    public async Task<ActionResult<List<OrderSummaryDto>>> ListOrders()
    {
        var result = await _mediator.Send(new ListOrdersQuery());
        return Ok(result);
    }
}