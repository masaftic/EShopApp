namespace EShopApp.Application.Payments.DTOs;

public record PaymentStatusResponse(string Id, PaymentStatus Status, string FailureReason);

public enum PaymentStatus
{
    Succeeded,
    Failed,
    Processing,
    Unknown
}
