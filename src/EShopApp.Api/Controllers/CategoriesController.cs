using EShopApp.Api.Models.Requests;
using EShopApp.Application.Categories.Commands;
using EShopApp.Application.Categories.Commands.Add;
using EShopApp.Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class CategoriesController : ApiController
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }


    [HttpGet("{categoryId:int}")]
    public async Task<IActionResult> GetById(int categoryId)
    {
        var query = new GetCategoryByIdQuery(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }
    
    [HttpGet("{categoryId:int}/tree")]
    public async Task<IActionResult> GetCategoryTree(int categoryId)
    {
        var query = new GetCategoryTreeByIdQuery(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("{categoryId:int}/descendants")]
    public async Task<IActionResult> GetCategoryDescendants(int categoryId)
    {
        var query = new GetCategoryDescendantsQuery(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("{categoryId:int}/subcategories")]
    public async Task<IActionResult> GetSubcategories(int categoryId)
    {
        var query = new GetSubCategoriesQuery(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpGet("{categoryId:int}/breadcrumbs")]
    public async Task<IActionResult> GetBreadCrumbs(int categoryId)
    {
        var query = new GetCategoryBreadCrumbsQuery(categoryId);
        var result = await _mediator.Send(query);

        return result.Match(Ok, HandleErrors);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetById), new { categoryId = value.Id }, value),
            HandleErrors
        );
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(DeleteCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(value => NoContent(), HandleErrors);
    }
}