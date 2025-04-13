using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EShopApp.Application.Payments.Events;

public class PaymentProcessingHandler : INotificationHandler<PaymentProcessingEvent>
{
    private readonly ILogger<PaymentProcessingHandler> _logger;
    private readonly IApplicationDbContext _dbContext;

    public PaymentProcessingHandler(ILogger<PaymentProcessingHandler> logger, IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Handle(PaymentProcessingEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment Processing: {PaymentIntentId}", notification.PaymentIntentId);

        var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.PaymentIntentId == notification.PaymentIntentId, cancellationToken);

        if (reservation is null)
        {
            _logger.LogError("No reservation found for PaymentIntentId: {PaymentIntentId}", notification.PaymentIntentId);

            throw new Exception($"no reservation found for {notification.PaymentIntentId}");
        }

        reservation.Status = ReservationStatus.AwaitingConfirmation;
        reservation.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Set order status to Processing in database
    }
}
