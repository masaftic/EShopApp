using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Products.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries;

public record GetBestSellingProductsQuery(int PageNumber = 1, int PageSize = 10) 
    : IRequest<ErrorOr<PaginatedList<ProductPreviewDto>>>;

public class GetBestSellingProductsQueryHandler 
    : IRequestHandler<GetBestSellingProductsQuery, ErrorOr<PaginatedList<ProductPreviewDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IImageStorageService _imageStorageService;

    public GetBestSellingProductsQueryHandler(IApplicationDbContext dbContext, IImageStorageService imageStorageService)
    {
        _dbContext = dbContext;
        _imageStorageService = imageStorageService;
    }

    public async Task<ErrorOr<PaginatedList<ProductPreviewDto>>> Handle(
        GetBestSellingProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products
            .OrderByDescending(p => p.SoldAmount)
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize);

        var totalCount = await _dbContext.Products.CountAsync(cancellationToken);
        
        var products = await query
            .Select(p => new ProductPreviewDto()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ThumbnailUrl = p.Images
                    .Where(i => i.IsMain)
                    .Select(i => _imageStorageService.GetPresignedUrl(i.ImageKey, ImageConstants.PresignedUrlExpiry))
                    .FirstOrDefault(),
                CategoryName = p.Category.Name,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductPreviewDto>(
            products, totalCount, request.PageSize, request.PageNumber);
    }
}
