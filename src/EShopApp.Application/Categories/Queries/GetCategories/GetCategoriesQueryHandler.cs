using ErrorOr;
using EShopApp.Application.Common.Helpers;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ErrorOr<List<Category>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly CategoryPathProcessor _categoryPathProcessor;
    public GetCategoriesQueryHandler(IApplicationDbContext dbContext, CategoryPathProcessor categoryPathProcessor)
    {
        _categoryPathProcessor = categoryPathProcessor;
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<List<Category>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Segments == null)
            return await _dbContext.Categories.ToListAsync(cancellationToken: cancellationToken);
        
        return await _categoryPathProcessor.ProcessSegmentsAsync(request.Segments);
    }
}