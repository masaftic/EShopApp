namespace EShopApp.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    public Money()
    {
    }

    public Money(decimal amount, string currency)
    {
        // if (amount < 0)
        //     throw new ArgumentException("Value must be non-negative.");
        Amount = amount;
        Currency = currency;
    }
}