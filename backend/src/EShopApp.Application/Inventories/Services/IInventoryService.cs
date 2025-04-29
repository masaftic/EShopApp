using ErrorOr;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Inventories.Services;

public interface IInventoryService
{
    Task<ErrorOr<Success>> AdjustInventoryAsync(
        IList<(int productId,
        int quantity)> productQuantities,
        string reason,
        InventoryTransactionType transactionType,
        CancellationToken cancellationToken);
    
    Task<ErrorOr<Success>> CheckStocks(
        IList<(int productId,
        int quantity)> productQuantities,
        CancellationToken cancellationToken);
}