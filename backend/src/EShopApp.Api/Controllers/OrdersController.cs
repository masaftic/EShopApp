using EShopApp.Application.Orders.Commands;
using EShopApp.Application.Orders.Queries;
using EShopApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class OrdersController : ApiController
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("track/{id:int}")]
    public async Task<IActionResult> TrackOrder(int id)
    {
        var query = new TrackOrderQuery(id);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var query = new GetOrderByUser();
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders()
    {
        var query = new GetAllOrdersQuery();
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus orderStatus)
    {
        var command = new UpdateOrderStatusCommand(id, orderStatus);
        var result = await _mediator.Send(command);

        return result.Match(res => NoContent(), HandleErrors);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var command = new CancelOrderCommand(id);
        var result = await _mediator.Send(command);

        return result.Match(res => NoContent(), HandleErrors);
    }
}