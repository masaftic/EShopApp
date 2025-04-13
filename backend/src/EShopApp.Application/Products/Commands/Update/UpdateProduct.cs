using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace EShopApp.Application.Products.Commands.Update;

public record UpdateProductCommand(
    int Id,
    string Name,
    decimal Price,
    string Description,
    int CategoryId) : IRequest<ErrorOr<ProductDto>>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<ProductDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateProductCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync([request.CategoryId], cancellationToken);
        if (category == null)
            return DomainErrors.Category.NotFound(request.CategoryId);

        var product = await _dbContext.Products.FindAsync([request.Id], cancellationToken);
        if (product == null)
            return DomainErrors.Product.NotFound;

        product.UpdateProduct(request.Name, request.Price,
            request.Description, request.CategoryId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}