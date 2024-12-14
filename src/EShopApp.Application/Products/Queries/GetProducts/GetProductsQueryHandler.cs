using System.Diagnostics;
using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Helpers;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ErrorOr<PaginatedList<Product>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly CategoryPathProcessor _categoryPathProcessor;
    
    public GetProductsQueryHandler(IApplicationDbContext dbContext, CategoryPathProcessor categoryPathProcessor)
    {
        _dbContext = dbContext;
        _categoryPathProcessor = categoryPathProcessor;
    }

    public async Task<ErrorOr<PaginatedList<Product>>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId);
        }
        else if (request.Segments is not null)
        {
            var subCategoriesResult = await _categoryPathProcessor.ProcessSegmentsAsync(request.Segments);
            if (subCategoriesResult.IsError)
                return subCategoriesResult.Errors;
            
            var subCategoriesIds = subCategoriesResult.Value.Select(c => c.Id).ToList();
            
            query = query.Where(p => subCategoriesIds.Contains(p.CategoryId));
        }

        if (request.MinPrice is not null)
            query = query.Where(p => p.Price.Amount >= request.MinPrice);

        if (request.MaxPrice is not null)
            query = query.Where(p => p.Price.Amount <= request.MaxPrice);

        query = query
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize);

        var products = await query.ToListAsync(cancellationToken: cancellationToken);

        return new PaginatedList<Product>(products, request.PageSize, request.PageNumber);
    }
}