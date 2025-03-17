using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProductByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Where(p => p.Id == request.Id)
            .ProjectToType<ProductDto>()
            .FirstOrDefaultAsync(cancellationToken);
        
        if (product is null)
            return Errors.Product.NotFound;

        return product;
    }
}
