using EShopApp.Application.Categories.Commands.Add;
using EShopApp.Application.Categories.Queries.GetAllCategories;
using EShopApp.Application.Categories.Queries.GetCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopApp.Api.Controllers;

[Route("api/[controller]")]
public class CategoryController : ApiController
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> Get(Guid categoryId)
    {
        var query = new GetCategoryQuery(categoryId);
        var result = await _mediator.Send(query);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Match(
            success => Ok(result.Value),
            errors => HandleErrors(errors)
        );
    }
}
