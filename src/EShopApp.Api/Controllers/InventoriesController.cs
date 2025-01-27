using ErrorOr;
using EShopApp.Api.Models.Requests;
using EShopApp.Application.Inventories.Commands.AddInventory;
using EShopApp.Application.Inventories.Commands.AdjustInventory;
using EShopApp.Application.Inventories.Queries.GetInventory;
using EShopApp.Application.Inventories.Queries.GetLowStocksInventories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class InventoriesController : ApiController
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllInventoriesQuery();
        var result = await _mediator.Send(query);
        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("{inventoryId:int}")]
    public async Task<IActionResult> GetById(int inventoryId)
    {
        var query = new GetInventoryByIdQuery(inventoryId);
        var result = await _mediator.Send(query);
        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var query = new GetLowStocksInventoriesQuery();
        var result = await _mediator.Send(query);
        return result.Match(Ok, HandleErrors);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddInventoryCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(
            inventory => CreatedAtAction(nameof(GetById), new { inventoryId = inventory.Id }, inventory),
            HandleErrors);
    }

    [HttpPost("{inventoryId:int}/adjust")]
    public async Task<IActionResult> Adjust(int inventoryId, [FromBody] AdjustInventoryRequest request)
    {
        var command =
            new AdjustInventoryCommand(inventoryId, request.AdjustmentType, request.Quantity, request.Reason);

        var result = await _mediator.Send(command);
        return result.Match(
            value => NoContent(),
            HandleErrors);
    }

    [HttpPut("{inventoryId:int}")]
    public async Task<IActionResult> Update(int inventoryId, [FromBody] UpdateInventoryRequest request)
    {
        var command = new UpdateInventoryCommand(inventoryId, request.ProductId, request.ReorderLevel,
            request.ReorderQuantity);

        var result = await _mediator.Send(command);
        return result.Match(
            value => NoContent(),
            HandleErrors);
    }
}