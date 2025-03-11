using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
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

        var payment = new Payment
        {
            UserId = reservation.UserId,
            // OrderId = orderId,
            PaymentIntentId = paymentIntent.PaymentIntentId,
            Amount = paymentIntent.AmountReceived / 100, // Convert from cents
            Currency = paymentIntent.Currency,
            Status = "succeeded",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Update order in database (set status to Paid)
    }
}
