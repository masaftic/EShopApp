using MediatR;

namespace EShopApp.Domain.Events;

public record PaymentProcessingEvent(string PaymentIntentId) : INotification;
