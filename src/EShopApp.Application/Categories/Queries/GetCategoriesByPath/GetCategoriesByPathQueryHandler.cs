using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategoriesByPath;

public class GetCategoriesByPathQueryHandler : IRequestHandler<GetCategoriesByPathQuery, ErrorOr<List<Category>>>
{
    private readonly IApplicationDbContext _dbContext;


    public GetCategoriesByPathQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<Category>>> Handle(GetCategoriesByPathQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
