using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Inventories.Commands.AdjustInventory;

public class AdjustInventoryHandler : IRequestHandler<AdjustInventoryCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;

    public AdjustInventoryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.Inventories.FindAsync([request.InventoryId], cancellationToken);
        if (inventory is null)
            return Error.NotFound(description: "Inventory not found");

        switch (request.TransactionType)
        {
            case InventoryTransactionType.Inbound:
                inventory.Stock += request.Quantity;
                break;
            case InventoryTransactionType.Adjustment:
                inventory.Stock = request.Quantity;
                break;
            default:
                return Error.Unexpected(description: "Transaction type not supported");
        }
        
        var inventoryTransaction = new InventoryTransaction(request.InventoryId, request.Quantity,
            request.TransactionType, DateTime.UtcNow, request.Reason);
        
        await _dbContext.InventoryTransactions.AddAsync(inventoryTransaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}