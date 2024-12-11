using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, ErrorOr<Category>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoryByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Category>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id, cancellationToken);
        if (category is null)
            return Errors.Category.NotFound(request.Id);

        return category;
    }
}