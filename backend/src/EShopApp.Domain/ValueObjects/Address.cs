
namespace EShopApp.Domain.ValueObjects;

public class Address : ValueObject
{
    public string StreetLine1 { get; private set; }
    public string StreetLine2 { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string PostalCode { get; private set; }

    private Address()
    {
        StreetLine1 = string.Empty;
        StreetLine2 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Country = string.Empty;
        PostalCode = string.Empty;
    }

    public Address(string streetLine1, string streetLine2, string city, string state, string country, string postalCode)
    {
        StreetLine1 = streetLine1;
        StreetLine2 = streetLine2;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    public static Address Default => new Address(
        "N/A",
        string.Empty,
        "N/A",
        "N/A",
        "N/A",
        "N/A"
    );

    public bool IsComplete()
    {
        return !string.IsNullOrWhiteSpace(StreetLine1) &&
               !string.IsNullOrWhiteSpace(City) &&
               !string.IsNullOrWhiteSpace(State) &&
               !string.IsNullOrWhiteSpace(Country) &&
               !string.IsNullOrWhiteSpace(PostalCode);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetLine1;
        yield return StreetLine2;
        yield return City;
        yield return State;
        yield return Country;
        yield return PostalCode;
    }
}