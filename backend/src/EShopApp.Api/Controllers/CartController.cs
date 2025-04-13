using EShopApp.Api.Models.Requests;
using EShopApp.Application.Carts.Commands.AddToCart;
using EShopApp.Application.Carts.Commands.Checkout;
using EShopApp.Application.Carts.Commands.ClearCart;
using EShopApp.Application.Carts.Commands.UpdateCart;
using EShopApp.Application.Carts.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class CartController : ApiController
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var getCartQuery = new GetCartQuery();
        var result = await _mediator.Send(getCartQuery);
        return result.Match(Ok, HandleErrors);
    }


    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart([FromBody] AddCartItemRequest request)
    {
        var addToCartCommand = new AddToCartCommand(request.ProductId, request.Quantity);
        var result = await _mediator.Send(addToCartCommand);
        return result.Match(Ok, HandleErrors);
    }

    [HttpDelete("clear-cart")]
    public async Task<IActionResult> ClearCart()
    {
        var clearCartCommand = new ClearCartCommand();
        var result = await _mediator.Send(clearCartCommand);
        return result.Match(value => NoContent(), HandleErrors);
    }

    [HttpPut("update-cart")]
    public async Task<IActionResult> UpdateCart([FromBody] UpdateCartCommand request)
    {
        var result = await _mediator.Send(request);
        return result.Match(Ok, HandleErrors);
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var checkoutCommand = new CheckoutCommand();
        var result = await _mediator.Send(checkoutCommand);
        return result.Match(Ok, HandleErrors); // payment intent result
    }
}