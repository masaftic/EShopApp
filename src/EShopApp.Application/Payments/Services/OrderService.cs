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

    public async Task<Order> PlaceOrderAsync(Reservation reservation, Payment payment, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            UserId = reservation.UserId,
            ReservationId = reservation.Id,
            PaymentId = payment.Id,
            OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
            TotalAmount = payment.Amount,
            Status = OrderStatus.Placed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = reservation.ReservationItems.Select(ri => new OrderItem
            {
                ProductId = ri.ProductId,
                Quantity = ri.Quantity,
                UnitPrice = ri.UnitPrice
            }).ToList(),
        };

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