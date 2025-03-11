using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using EShopApp.Domain.Events;
using MediatR;

namespace EShopApp.Application.Payments.Events;

public class StripeWebhookReceivedHandler : INotificationHandler<StripeWebhookReceivedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly IMediator _mediator;

    public StripeWebhookReceivedHandler(IPaymentService paymentService, IMediator mediator)
    {
        _paymentService = paymentService;
        _mediator = mediator;
    }

    public async Task Handle(StripeWebhookReceivedEvent notification, CancellationToken cancellationToken)
    {
        var processedPaymentResult = await _paymentService.ProcessWebhookAsync(notification.RawJson, notification.Signature);
        if (processedPaymentResult.IsError)
        {
            await _mediator.Publish(new PaymentFailedEvent(null, processedPaymentResult.Errors), cancellationToken);
            return;
        }

        var processedPayment = processedPaymentResult.Value;

        if (processedPayment.Status == PaymentStatus.Succeeded)
        {
            await _mediator.Publish(new PaymentSucceededEvent(processedPayment.Id), cancellationToken);
        }
        else if (processedPayment.Status == PaymentStatus.Processing)
        {
            await _mediator.Publish(new PaymentProcessingEvent(processedPayment.Id), cancellationToken);
        }
        else if (processedPayment.Status == PaymentStatus.Failed)
        {
            await _mediator.Publish(new PaymentFailedEvent(processedPayment.Id, Error.Conflict(description: processedPayment.FailureReason)), cancellationToken);
        }
    }
}
