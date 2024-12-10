using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Domain.ValueObjects;
using MediatR;

namespace EShopApp.Application.Products.Commands.Update;

public record UpdateProductCommand(
    int Id,
    string Name,
    int Quantity,
    decimal PriceAmount,
    string PriceCurrency,
    string Description,
    int CategoryId) : IRequest<ErrorOr<Product>>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<Product>>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateProductCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Errors.Category.NotFound;

        var product = await _dbContext.Products.FindAsync(request.Id, cancellationToken);
        if (product == null)
            return Errors.Product.NotFound;

        product.UpdateProduct(request.Name, request.Quantity, new Money(request.PriceAmount, request.PriceCurrency),
            request.Description, request.CategoryId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }
}