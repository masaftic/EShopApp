using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Payments.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _dbContext;

    public OrderService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order> PlaceOrderAsync(int userId, Reservation reservation, Payment payment, CancellationToken cancellationToken)
    {
        var order = new Order(userId, reservation.Id, payment.Id, payment.Amount);

        var items = reservation.ReservationItems.Select(ri => (ri.ProductId, ri.Quantity, ri.UnitPrice));

        order.AddItems(items);

        var productIds = reservation.ReservationItems.Select(ri => ri.ProductId).ToList();
        var inventories = await _dbContext.Inventories
            .Where(i => productIds.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId, i => i, cancellationToken);

        var inventoryTransactions = new List<InventoryTransaction>();
        foreach (var ri in reservation.ReservationItems)
        {
            var inventory = inventories[ri.ProductId];
            inventory.DecreaseStock(ri.Quantity);

            inventoryTransactions.Add(new InventoryTransaction
            {
                InventoryId = inventory.Id,
                Quantity = -ri.Quantity,
                TransactionType = InventoryTransactionType.Outbound,
                Timestamp = DateTime.UtcNow,
                Reason = "Order Placed"
            });
        }

        _dbContext.InventoryTransactions.AddRange(inventoryTransactions);
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}