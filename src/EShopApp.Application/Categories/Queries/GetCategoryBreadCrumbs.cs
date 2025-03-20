using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EShopApp.Domain.Errors;

namespace EShopApp.Application.Categories.Queries;

public record GetCategoryBreadCrumbsQuery(
    int CategoryId) : IRequest<ErrorOr<List<CategoryDto>>>;

public class GetCategoryBreadCrumbsQueryHandler : IRequestHandler<GetCategoryBreadCrumbsQuery, ErrorOr<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoryBreadCrumbsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<CategoryDto>>> Handle(GetCategoryBreadCrumbsQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync([request.CategoryId], cancellationToken);

        if (category is null)
            return Errors.Category.NotFound(request.CategoryId);

        var ids = category.Path.Split('/').Where(id => !string.IsNullOrWhiteSpace(id)).Select(int.Parse).ToList();

        return await _dbContext
            .Categories
            .ProjectToType<CategoryDto>()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }
}
;