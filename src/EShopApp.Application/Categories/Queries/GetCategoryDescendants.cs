using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EShopApp.Domain.Errors;

namespace EShopApp.Application.Categories.Queries;

public record GetCategoryDescendantsQuery(
    int CategoryId) : IRequest<ErrorOr<List<CategoryDto>>>;

public class GetCategoryDescendantsQueryHandler : IRequestHandler<GetCategoryDescendantsQuery, ErrorOr<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoryDescendantsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<CategoryDto>>> Handle(GetCategoryDescendantsQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync([request.CategoryId], cancellationToken);

        if (category is null)
            return DomainErrors.Category.NotFound(request.CategoryId);

        return await _dbContext.Categories
            .Where(c => c.Path.StartsWith($"{category.Path}/")) 
            // trailing '/' is for ids with more than one digit. ex /1/3 and /1/33
            .ProjectToType<CategoryDto>()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
