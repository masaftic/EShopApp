using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries.GetAllProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ErrorOr<PaginatedList<Product>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProductsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<PaginatedList<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsQueryable();
        
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);
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