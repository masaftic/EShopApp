namespace EShopApp.Domain.ValueObjects;

public class Address
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }

    public Address(string street, string city, string country)
    {
        Street = street;
        City = city;
        Country = country;
    }

    public override string ToString() => $"{Street}, {City}, {Country}";
}