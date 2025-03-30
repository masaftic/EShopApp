using System.Diagnostics;
using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ErrorOr<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    
    public GetProductsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaginatedList<ProductDto>>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId);
        }

        if (request.MinPrice is not null)
            query = query.Where(p => p.Price >= request.MinPrice);

        if (request.MaxPrice is not null)
            query = query.Where(p => p.Price <= request.MaxPrice);

        query = request.SortBy switch
        {
            "name" => request.SortOrder switch
            {
                "asc" => query.OrderBy(p => p.Name),
                "desc" => query.OrderByDescending(p => p.Name),
                _ => query
            },
            "price" => request.SortOrder switch
            {
                "asc" => query.OrderBy(p => p.Price),
                "desc" => query.OrderByDescending(p => p.Price),
                _ => query
            },
            _ => query
        };

        query = query
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize);

        var totalCount = await _dbContext.Products.CountAsync(cancellationToken: cancellationToken);
        
        var products = await query
            .ProjectToType<ProductDto>()
            .ToListAsync(cancellationToken: cancellationToken);
        
        return new PaginatedList<ProductDto>(products, totalCount, request.PageSize, request.PageNumber);
    }
}