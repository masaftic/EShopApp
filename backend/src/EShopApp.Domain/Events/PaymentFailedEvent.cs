using ErrorOr;
using MediatR;

namespace EShopApp.Domain.Events;

public record PaymentFailedEvent(string? PaymentIntentId, ErrorOr<Success> Error) : INotification;