using EShopApp.Api.Models.Requests;
using EShopApp.Application.Products.Commands.Add;
using EShopApp.Application.Products.Queries.GetAllProducts;
using EShopApp.Application.Products.Queries.GetProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class ProductController : ApiController
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllProductsQuery();
        var result = await _mediator.Send(query);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> Get(Guid productId)
    {
        var query = new GetProductQuery(productId);
        var result = await _mediator.Send(query);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }

    [HttpPost("{categoryId:guid}")]
    public async Task<IActionResult> AddProduct(AddProductRequest request, Guid categoryId)
    {
        var command = new AddProductCommand(request.Name, request.Quantity, request.PriceAmount, request.PriceCurrency,
            request.Description, categoryId);
        var result = await _mediator.Send(command);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }
}