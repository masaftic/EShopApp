using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Domain.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands.Add;


public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ErrorOr<ProductDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public AddProductCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ErrorOr<ProductDto>> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(command.CategoryId);
        if (category == null)
            return Errors.Category.NotFound(command.CategoryId);
        
        var product = new Product(command.Name, command.Price, command.Description, command.CategoryId);
        
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Adapt<ProductDto>();
    }
}
