using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries;

public record GetSubCategoriesQuery(
    int CategoryId) : IRequest<ErrorOr<List<CategoryDto>>>;

public class GetSubCategoriesQueryHandler : IRequestHandler<GetSubCategoriesQuery, ErrorOr<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetSubCategoriesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<List<CategoryDto>>> Handle(GetSubCategoriesQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!category)
            return DomainErrors.Category.NotFound(request.CategoryId);
    
        return await _dbContext.Categories
            .Where(c => c.ParentId == request.CategoryId)
            .ProjectToType<CategoryDto>()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}