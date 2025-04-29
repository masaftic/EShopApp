using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Services;

public class InventoryService : IInventoryService
{
    private readonly IApplicationDbContext _dbContext;

    public InventoryService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<Success>> AdjustInventoryAsync(IList<(int productId, int quantity)> productQuantities, string reason, InventoryTransactionType transactionType, CancellationToken cancellationToken)
    {
        var productIds = productQuantities.Select(pq => pq.productId).ToList();
        var inventories = await _dbContext.Inventories.Where(i => productIds.Contains(i.ProductId)).ToDictionaryAsync(i => i.ProductId, i => i, cancellationToken);
        var inventoryTransactions = new List<InventoryTransaction>();
        foreach (var (productId, quantity) in productQuantities)
        {
            if (!inventories.TryGetValue(productId, out var inventory))
            {
                return Error.NotFound(description: $"No inventory found for product: {productId}");
            }

            switch (transactionType)
            {
                case InventoryTransactionType.Release:
                    inventory.Release(quantity);
                    break;
                case InventoryTransactionType.Reserve:
                    if (inventory.AvailableStock < quantity)
                    {
                        return DomainErrors.Inventory.InsufficientStock(productId);
                    }
                    inventory.Reserve(quantity);
                    break;
                case InventoryTransactionType.Outbound:
                    if (inventory.Stock < quantity)
                    {
                        return DomainErrors.Inventory.InsufficientStock(productId);
                    }
                    inventory.DecreaseStock(quantity);
                    break;
                case InventoryTransactionType.Inbound:
                    inventory.IncreaseStock(quantity);
                    break;
                default:
                    return Error.Failure(description: $"Unknown transaction type: {transactionType}");
            }

            inventoryTransactions.Add(new InventoryTransaction
            (inventory.Id, quantity, transactionType, DateTime.UtcNow, reason));
        }

        await _dbContext.InventoryTransactions.AddRangeAsync(inventoryTransactions, cancellationToken);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> CheckStocks(IList<(int productId,
        int quantity)> productQuantities, CancellationToken cancellationToken)
    {
        var productIds = productQuantities.Select(pq => pq.productId).ToList();
        var inventories = _dbContext.Inventories.Where(i => productIds.Contains(i.ProductId));

        var insufficientStock = await inventories
            .Where(i => i.AvailableStock < productQuantities.FirstOrDefault(pq => pq.productId == i.ProductId).quantity)
            .Select(i => DomainErrors.Inventory.InsufficientStock(i.ProductId))
            .ToListAsync();

        if (insufficientStock.Count != 0)
        {
            return insufficientStock;
        }

        return Result.Success;
    }
}
