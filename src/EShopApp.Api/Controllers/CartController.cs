using EShopApp.Api.Models.Requests;
using EShopApp.Application.ShoppingCarts.Commands.AddToCart;
using EShopApp.Application.ShoppingCarts.Commands.ClearCart;
using EShopApp.Application.ShoppingCarts.Queries.Get;
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
        return ToOkOrErrors(result);
    }
    

    [HttpPost("add-to-cart")]
    public async Task<IActionResult> AddToCart([FromBody] AddCartItemRequest request)
    {
        var addToCartCommand = new AddToCartCommand(request.ProductId, request.Quantity);
        var result = await _mediator.Send(addToCartCommand);
        return ToOkOrErrors(result);
    }
    
    [HttpPost("clear-cart")]
    public async Task<IActionResult> ClearCart()
    {
        var clearCartCommand = new ClearCartCommand();
        var result = await _mediator.Send(clearCartCommand);
        return ToOkOrErrors(result);
    }
}