using EShopApp.Api.Models.Requests;
using EShopApp.Application.Products.Commands;
using EShopApp.Application.Products.Commands.Add;
using EShopApp.Application.Products.Commands.Delete;
using EShopApp.Application.Products.Commands.Update;
using EShopApp.Application.Products.Queries;
using EShopApp.Application.Products.Queries.GetProductById;
using Mapster;
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
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery request)
    {
        var result = await _mediator.Send(request);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("best-selling")]
    public async Task<IActionResult> GetBestSellingProducts([FromQuery] GetBestSellingProductsQuery request)
    {
        var result = await _mediator.Send(request);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRatedProducts([FromQuery] GetTopRatedProductsQuery request)
    {
        var result = await _mediator.Send(request);

        return result.Match(Ok, HandleErrors);
    }


    [HttpGet("filter")]
    public async Task<IActionResult> GetFilteredProducts([FromQuery] GetFilteredProductsQuery request)
    {
        var result = await _mediator.Send(request);

        return result.Match(Ok, HandleErrors);
    }


    [HttpGet("{productId:int}")]
    public async Task<IActionResult> GetProductById(int productId)
    {
        var query = new GetProductByIdQuery(productId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddProduct(AddProductRequest request)
    {
        var command = request.Adapt<AddProductCommand>();

        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetProductById), new { productId = value.Id }, value),
            HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> DeleteProduct(int productId)
    {
        var command = new DeleteProductCommand(productId);
        var result = await _mediator.Send(command);

        return result.Match(value => NoContent(), HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
    {
        var command = new UpdateProductCommand(id, request.Name, request.Price,
            request.Description, request.CategoryId);

        var result = await _mediator.Send(command);

        return result.Match(Ok, HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{productId:int}/images")]
    public async Task<IActionResult> AddImageToProduct(int productId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        if (file.Length > 5 * 1024 * 1024) // 5 MB limit
            return BadRequest("File size exceeds the limit of 5 MB.");

        var allowedImageTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp", "image/tiff" };

        if (!allowedImageTypes.Contains(file.ContentType))
            return BadRequest("Invalid file type. Only JPEG, PNG, GIF, WEBP, BMP, and TIFF images are allowed.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var command = new AddImageToProductCommand(productId, file.FileName, file.ContentType, memoryStream.ToArray());

        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetProductById), new { productId }, null), 
            HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{productId:int}/images/{imageId:int}/main")]
    public async Task<IActionResult> SetImageAsMain(int productId, int imageId)
    {
        var command = new SetImageAsMainCommand(productId, imageId);
        var result = await _mediator.Send(command);

        return result.Match(res => NoContent(), HandleErrors);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{productId:int}/images/{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int productId, int imageId)
    {
        var command = new RemoveImageFromProductCommand(productId, imageId);
        var result = await _mediator.Send(command);

        return result.Match(res => NoContent(), HandleErrors);
    }

    [Authorize]
    [HttpPost("{productId:int}/reviews")]
    public async Task<IActionResult> AddReview(int productId, AddReviewRequest request)
    {
        var command = new AddReviewToProductCommand(
            productId,
            request.Comment,
            request.Rating);

        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetProductById), new { productId }, null),
            HandleErrors);
    }

    [Authorize]
    [HttpPut("{productId:int}/reviews/{reviewId:int}")]
    public async Task<IActionResult> UpdateReview(
        int productId,
        int reviewId,
        UpdateReviewRequest request)
    {
        var command = new UpdateProductReviewCommand(
            productId,
            reviewId,
            request.Comment,
            request.Rating);
            
        var result = await _mediator.Send(command);

        return result.Match(
            value => NoContent(),
            HandleErrors);
    }

    [Authorize]
    [HttpDelete("{productId:int}/reviews/{reviewId:int}")]
    public async Task<IActionResult> DeleteReview(int productId, int reviewId)
    {
        var command = new DeleteReviewFromProductCommand(productId, reviewId);
        var result = await _mediator.Send(command);

        return result.Match(
            value => NoContent(),
            HandleErrors);
    }
}