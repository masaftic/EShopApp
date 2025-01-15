using EShopApp.Api.Models.Requests;
using EShopApp.Application.Categories.Commands.Add;
using EShopApp.Application.Categories.Queries.GetCategories;
using EShopApp.Application.Categories.Queries.GetCategoryById;
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
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesRequest request)
    {
        var segments = request.Path?.Split("/", StringSplitOptions.RemoveEmptyEntries);
        
        var query = new GetCategoriesQuery(segments);
        var result = await _mediator.Send(query);

        return ToOkOrErrors(result);
    }


    [HttpGet("{categoryId:int}")]
    public async Task<IActionResult> GetById(int categoryId)
    {
        var query = new GetCategoryByIdQuery(categoryId);
        var result = await _mediator.Send(query);

        return ToOkOrErrors(result);
    }
    

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return result.Match(
            value => CreatedAtAction(nameof(GetById), new { categoryId = value.Id }, value),
            errors => HandleErrors(errors)
        );
    }
}