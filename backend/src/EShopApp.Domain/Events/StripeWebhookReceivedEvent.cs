using MediatR;

namespace EShopApp.Domain.Events;

public record StripeWebhookReceivedEvent(string RawJson, string Signature) : INotification;