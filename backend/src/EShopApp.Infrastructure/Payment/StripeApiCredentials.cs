namespace EShopApp.Infrastructure.Payment;

public class StripeApiCredentials
{
    public const string SectionName = "Stripe";
    public string SecretKey { get; set; } = "";
    public string PublishableKey { get; set; } = "";
    public string WebhookSecret { get; set; } = "";
}

