using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
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
        var product = new Product(Guid.NewGuid(), request.Name, new Money(request.PriceAmount, request.PriceCurrency), request.Description, request.CategoryId);
        
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }
}
