using MediatR;

namespace EShopApp.Domain.Events;

public record PaymentSucceededEvent(string PaymentIntentId) : INotification;
