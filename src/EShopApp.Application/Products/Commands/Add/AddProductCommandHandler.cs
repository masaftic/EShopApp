using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Domain.ValueObjects;
using MediatR;

namespace EShopApp.Application.Products.Commands.Add;


public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ErrorOr<Product>>
{
    private readonly IApplicationDbContext _dbContext;

    public AddProductCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<Product>> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.CategoryId, cancellationToken);
        if (category == null)
            return Errors.Category.NotFound;
        
        var product = new Product(request.Name, new Money(request.PriceAmount, request.PriceCurrency), request.Description, request.CategoryId);
        
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }
}
