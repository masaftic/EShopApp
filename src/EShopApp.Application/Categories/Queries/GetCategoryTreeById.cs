using Dumpify;
using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries;

public record GetCategoryTreeByIdQuery(
    int CategoryId) : IRequest<ErrorOr<CategoryTreeDto>>;

public class GetCategoryTreeByIdQueryHandler : IRequestHandler<GetCategoryTreeByIdQuery, ErrorOr<CategoryTreeDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMediator _mediator;

    public GetCategoryTreeByIdQueryHandler(IApplicationDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }


    public async Task<ErrorOr<CategoryTreeDto>> Handle(GetCategoryTreeByIdQuery request, CancellationToken cancellationToken)
    {
        var getCategoryDescendantsResult = await _mediator.Send(new GetCategoryDescendantsQuery(request.CategoryId), cancellationToken);

        if (getCategoryDescendantsResult.IsError)
            return getCategoryDescendantsResult.Errors;

        var rootCategory = (await _dbContext.Categories.FindAsync([request.CategoryId], cancellationToken)).Adapt<CategoryDto>();

        var categoryDescendants = getCategoryDescendantsResult.Value;

        var graph = new Dictionary<int, List<CategoryDto>>();
        foreach (var category in categoryDescendants)
        {
            if (category.ParentId.HasValue)
            {
                if (graph.TryGetValue(category.ParentId.Value, out List<CategoryDto>? parent))
                {
                    parent.Add(category);
                }
                else
                {
                    graph[category.ParentId.Value] = [category];
                }
            }
        }

        return CategoryTreeDto.BuildTreeFromGraph(rootCategory, graph);
    }
}