namespace EShopApp.Domain.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }


    private Address()
    {
    }

    public Address(string street, string city, string country)
    {
        Street = street;
        City = city;
        Country = country;
    }

    public override string ToString() => $"{Street}, {City}, {Country}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
    }
}