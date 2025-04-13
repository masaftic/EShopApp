using MediatR;
using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Commands.AddInventory;


public class AddInventoryHandler : IRequestHandler<AddInventoryCommand, ErrorOr<InventoryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public AddInventoryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<InventoryDto>> Handle(AddInventoryCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync([request.ProductId], cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound;

        if (await _dbContext.Inventories.AnyAsync(i => i.ProductId == product.Id, cancellationToken))
        {
            return Error.Conflict(description: "Inventory already exists");
        }
        
        var inventory = new Inventory(product.Id, request.Stock, request.ReorderQuantity, request.ReorderLevel);
        await _dbContext.Inventories.AddAsync(inventory, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return inventory.Adapt<InventoryDto>();
    }
}