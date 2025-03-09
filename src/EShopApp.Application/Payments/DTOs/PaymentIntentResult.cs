namespace EShopApp.Application.Payments.Commands.CreatePayment;

public record PaymentIntentResult(string PaymentIntentId, string Status, string ClientSecret);
