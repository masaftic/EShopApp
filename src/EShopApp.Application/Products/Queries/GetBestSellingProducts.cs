using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries;

public record GetBestSellingProductsQuery(int PageNumber = 1, int PageSize = 10) 
    : IRequest<ErrorOr<PaginatedList<ProductDto>>>;

public class GetBestSellingProductsQueryHandler 
    : IRequestHandler<GetBestSellingProductsQuery, ErrorOr<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetBestSellingProductsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaginatedList<ProductDto>>> Handle(
        GetBestSellingProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .OrderByDescending(p => p.SoldAmount)
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize);

        var totalCount = await _dbContext.Products.CountAsync(cancellationToken);
        
        var products = await query
            .ProjectToType<ProductDto>()
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductDto>(
            products, totalCount, request.PageSize, request.PageNumber);
    }
}
