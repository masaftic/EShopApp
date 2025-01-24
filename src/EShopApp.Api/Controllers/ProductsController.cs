using EShopApp.Api.Mappers;
using EShopApp.Api.Models.Requests;
using EShopApp.Application.Products.Commands.Add;
using EShopApp.Application.Products.Commands.Delete;
using EShopApp.Application.Products.Commands.Update;
using EShopApp.Application.Products.Queries.GetProduct;
using EShopApp.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class ProductsController : ApiController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsRequest request)
    {
        if (request.Path is not null && request.CategoryId is not null)
        {
            return BadRequest("Filter either by CategoryId or by Path, but not both.");
        }

        var query = new GetProductsQuery(request.CategoryId, request.Path, request.MinPrice, request.MaxPrice,
            request.PageNumber,
            request.PageSize);

        var result = await _mediator.Send(query);

        return result.Match(
            value => Ok(value.ToPaginatedListResponse()),
            errors => HandleErrors(errors)
        );
    }

    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProductById(int productId)
    {
        var query = new GetProductQuery(productId);
        var result = await _mediator.Send(query);

        return result.Match(
            value => Ok(value.ToProductResponse()),
            errors => HandleErrors(errors)
        );
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{categoryId:int}")]
    public async Task<IActionResult> AddProduct(AddProductRequest request, int categoryId)
    {
        var command = new AddProductCommand(request.Name, request.Quantity, request.Price,
            request.Description, categoryId);
        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetProductById), new { productId = value.Id }, value.ToProductResponse()),
            errors => HandleErrors(errors));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var command = new DeleteProductCommand(productId);
        var result = await _mediator.Send(command);

        return ToNoContentOrErrors(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Quantity, request.Price,
            request.Description, request.CategoryId);

        var result = await _mediator.Send(command);

        return result.Match(
            value => Ok(value.ToProductResponse()),
            errors => HandleErrors(errors)
        );
    }
}

