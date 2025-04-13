using EShopApp.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EShopApp.Application.Payments.Events;

public class PaymentFailedHandler : INotificationHandler<PaymentFailedEvent>
{
    private readonly ILogger<PaymentFailedHandler> _logger;

    public PaymentFailedHandler(ILogger<PaymentFailedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentFailedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Payment Failed: {PaymentIntentId}, Reason: {FailureReason}", notification.PaymentIntentId, notification.Error.FirstError.Description);

        // TODO: Notify user or update order status to Failed

        return Task.CompletedTask;
    }
}
