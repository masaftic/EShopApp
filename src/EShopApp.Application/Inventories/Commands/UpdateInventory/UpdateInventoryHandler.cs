using MediatR;
using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Commands.UpdateInventory;

public record UpdateInventoryCommand(
    int InventoryId,
    int ProductId,
    int ReorderLevel,
    int ReorderQuantity) : IRequest<ErrorOr<Success>>;


public class UpdateInventoryHandler : IRequestHandler<UpdateInventoryCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateInventoryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.Inventories.FindAsync([request.InventoryId], cancellationToken);
        if (inventory is null)
            return Errors.Inventory.NotFound(request.InventoryId);
        
        var product = await _dbContext.Products.FindAsync([request.ProductId], cancellationToken);
        if (product is null)
            return Errors.Product.NotFound;

        if (await _dbContext.Inventories.AnyAsync(i => i.ProductId == product.Id, cancellationToken))
            return Error.Conflict(description: "Inventory already exists");
        
        inventory.ProductId = request.ProductId;
        inventory.ReorderLevel = request.ReorderLevel;
        inventory.ReorderQuantity = request.ReorderQuantity;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}