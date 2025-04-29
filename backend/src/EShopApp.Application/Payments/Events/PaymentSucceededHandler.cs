using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Enums;
using EShopApp.Domain.Events;
using EShopApp.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EShopApp.Application.Payments.Events;

public class PaymentSucceededHandler : INotificationHandler<PaymentSucceededEvent>
{
    private readonly ILogger<PaymentSucceededHandler> _logger;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;

    public PaymentSucceededHandler(ILogger<PaymentSucceededHandler> logger, IApplicationDbContext dbContext, IPaymentService paymentService, IOrderService orderService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _paymentService = paymentService;
        _orderService = orderService;
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
            throw new Exception($"No reservation found for {notification.PaymentIntentId}");
        }

        reservation.Status = ReservationStatus.Fulfilled;
        reservation.UpdatedAt = DateTime.UtcNow;

        var paymentIntentResult = await _paymentService.GetPaymentIntentAsync(notification.PaymentIntentId);

        if (paymentIntentResult.IsError)
        {
            _logger.LogError("Could not get paymentIntent for {PaymentIntentId}. Errors: {@Errors}", notification.PaymentIntentId, paymentIntentResult.Errors);
            throw new Exception($"Could not get paymentIntent for {notification.PaymentIntentId}");
        }

        var paymentIntent = paymentIntentResult.Value;

        var shippingAddress = paymentIntent.ShippingAddress ?? Address.Default;

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = reservation.UserId;

            var userCart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .FirstAsync(c => c.UserId == reservation.UserId, cancellationToken);

            userCart.ClearCart();
            _dbContext.Carts.Update(userCart);

            var payment = new Payment
            {
                UserId = userId,
                PaymentIntentId = paymentIntent.PaymentIntentId,
                Amount = paymentIntent.AmountReceived / 100, // Convert from cents
                Currency = paymentIntent.Currency,
                Status = "succeeded",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbContext.Payments.AddAsync(payment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var order = await _orderService.PlaceOrderAsync(userId, reservation, payment, shippingAddress, cancellationToken);

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
