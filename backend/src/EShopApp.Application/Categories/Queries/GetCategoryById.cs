using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Interfaces.Caching;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries;

public record GetCategoryByIdQuery(
    int Id) : ICachedQuery<ErrorOr<CategoryDto>>
{
    public string CacheKey => $"Category_{Id}";
    public TimeSpan CacheExpiration => TimeSpan.FromMinutes(10);
}

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, ErrorOr<CategoryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoryByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext
            .Categories
            .ProjectToType<CategoryDto>()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
            return DomainErrors.Category.NotFound(request.Id);

        return category;
    }
}