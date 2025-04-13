namespace EShopApp.Application.Payments.DTOs;

public record PaymentIntentResult(
    string PaymentIntentId,
    string Status,
    string ClientSecret,
    long Amount,
    long AmountReceived,
    string Currency,
    string Description,
    Dictionary<string, string> Metadata);
