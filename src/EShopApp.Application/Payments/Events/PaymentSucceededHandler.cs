using EShopApp.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EShopApp.Application.Payments.Events.PaymentSucceeded;

public class PaymentSucceededHandler : INotificationHandler<PaymentSucceededEvent>
{
    private readonly ILogger<PaymentSucceededHandler> _logger;

    public PaymentSucceededHandler(ILogger<PaymentSucceededHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment Succeeded: {PaymentIntentId}", notification.PaymentIntentId);

        // TODO: Update order in database (set status to Paid)

        return Task.CompletedTask;
    }
}
