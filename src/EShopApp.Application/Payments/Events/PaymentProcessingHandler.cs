using EShopApp.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

public class PaymentProcessingHandler : INotificationHandler<PaymentProcessingEvent>
{
    private readonly ILogger<PaymentProcessingHandler> _logger;

    public PaymentProcessingHandler(ILogger<PaymentProcessingHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(PaymentProcessingEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment Processing: {PaymentIntentId}", notification.PaymentIntentId);

        // TODO: Set order status to Processing in database

        return Task.CompletedTask;
    }
}
