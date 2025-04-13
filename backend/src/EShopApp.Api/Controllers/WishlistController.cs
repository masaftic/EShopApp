using EShopApp.Application.Wishlists.DTOs;
using EShopApp.Application.Wishlists.Commands;
using EShopApp.Application.Wishlists.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErrorOr;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class WishlistController : ApiController
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var result = await _mediator.Send(new GetWishlistQuery());
        return result.Match(Ok, HandleErrors);
    }

    [HttpPost("add-item")]
    public async Task<IActionResult> AddItemToWishlist([FromBody] AddItemToWishlistCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(success => NoContent(), HandleErrors);
    }

    [HttpDelete("remove-item/{productId}")]
    public async Task<IActionResult> RemoveItemFromWishlist(int productId)
    {
        var result = await _mediator.Send(new RemoveItemFromWishlistCommand(productId));
        return result.Match(success => NoContent(), HandleErrors);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearWishlist()
    {
        var result = await _mediator.Send(new ClearWishlistCommand());
        return result.Match(success => NoContent(), HandleErrors);
    }

    [HttpGet("check-item/{productId}")]
    public async Task<IActionResult> CheckIfItemInWishlist(int productId)
    {
        var result = await _mediator.Send(new CheckIfItemInWishlistQuery(productId));
        return result.Match(isInWishlist => Ok(new { IsInWishlist = isInWishlist }), HandleErrors);
    }
}

