namespace EShopApp.Application.Payments.DTOs;


public class PaymentIntentOptionsDto
{
    public long Amount { get; set; } // in smallest currency unit (e.g., cents for USD)
    public string Currency { get; set; } = "";
    public Dictionary<string, string> Metadata { get; set; } = [];
}