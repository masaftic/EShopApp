using EShopApp.Application.Orders.Commands;
using EShopApp.Application.Orders.Queries;
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

    [HttpPost("")]
    public async Task<IActionResult> PlaceOrder(PlaceOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(
            value => CreatedAtAction(nameof(GetById), new { id = value.Id }, value),
            HandleErrors);
    }
}