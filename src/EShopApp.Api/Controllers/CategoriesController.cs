using EShopApp.Application.Categories.Commands.Add;
using EShopApp.Application.Categories.Queries.GetAllCategories;
using EShopApp.Application.Categories.Queries.GetCategory;
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
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllCategoriesQuery();
        var result = await _mediator.Send(query);

        return ToOkOrErrors(result);
    }

    [HttpGet("{categoryId:int}")]
    public async Task<IActionResult> Get(int categoryId)
    {
        var query = new GetCategoryQuery(categoryId);
        var result = await _mediator.Send(query);

        return ToOkOrErrors(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(Get), new { categoryId = value.Id }, value),
            errors => HandleErrors(errors)
        );
    }
}