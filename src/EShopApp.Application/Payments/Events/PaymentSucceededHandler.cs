using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Enums;
using EShopApp.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EShopApp.Application.Payments.Events;

public class PaymentSucceededHandler : INotificationHandler<PaymentSucceededEvent>
{
    private readonly ILogger<PaymentSucceededHandler> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;

    public PaymentSucceededHandler(ILogger<PaymentSucceededHandler> logger, IApplicationDbContext dbContext, IPaymentService paymentService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _paymentService = paymentService;
    }

    public async Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment Succeeded: {PaymentIntentId}", notification.PaymentIntentId);

        var reservation = await _dbContext.Reservations
            .Include(r => r.ReservationItems)
            .FirstOrDefaultAsync(r => r.PaymentIntentId == notification.PaymentIntentId, cancellationToken);
    

        if (reservation is null)
        {
            _logger.LogError("No reservation found for PaymentIntentId: {PaymentIntentId}", notification.PaymentIntentId);

            throw new Exception($"no reservation found for {notification.PaymentIntentId}");
        }

        reservation.Status = ReservationStatus.Fulfilled;
        reservation.UpdatedAt = DateTime.UtcNow;

        var paymentIntentResult = await _paymentService.GetPaymentIntentAsync(notification.PaymentIntentId);

        if (paymentIntentResult.IsError)
        {
            _logger.LogError("Could not get paymentIntent for {PaymentIntentId}", notification.PaymentIntentId);

            throw new Exception($"Could not get paymentIntent for {notification.PaymentIntentId}");
        }

        var paymentIntent = paymentIntentResult.Value;

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userCart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.UserId == reservation.UserId, cancellationToken);
            
            userCart.ClearCart();
            _dbContext.Carts.Update(userCart);

            var payment = new Payment
            {
                UserId = reservation.UserId,
                PaymentIntentId = paymentIntent.PaymentIntentId,
                Amount = paymentIntent.AmountReceived / 100, // Convert from cents
                Currency = paymentIntent.Currency,
                Status = "succeeded",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

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

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Order placed successfully for PaymentIntentId: {PaymentIntentId}, With OrderId: {OrderId}", notification.PaymentIntentId, order.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        // TODO: Publish OrderPlacedEvent
    }
}
