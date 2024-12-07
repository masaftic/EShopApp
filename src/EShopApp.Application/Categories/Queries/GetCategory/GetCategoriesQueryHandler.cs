using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, ErrorOr<Category>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoryQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Category>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id, cancellationToken);
        if (category is null)
            return Errors.Category.NotFound;

        return category;
    }
}