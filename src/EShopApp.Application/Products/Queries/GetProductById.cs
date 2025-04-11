using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(
    int Id) : IRequest<ErrorOr<ProductDto>>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IImageStorageService _imageStorageService;

    public GetProductByIdQueryHandler(IApplicationDbContext dbContext, IImageStorageService imageStorageService)
    {
        _dbContext = dbContext;
        _imageStorageService = imageStorageService;
    }

    public async Task<ErrorOr<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Where(p => p.Id == request.Id)
            .ProjectToType<ProductDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;

        foreach (var image in product.Images)
        {
            image.ImageUrl = _imageStorageService.GetPresignedUrl(image.ImageUrl, TimeSpan.FromMinutes(15));
        }

        return product;
    }
}
