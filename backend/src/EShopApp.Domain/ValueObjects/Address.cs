namespace EShopApp.Domain.ValueObjects;

public class Address : ValueObject
{
    public string AddressLine1 { get; set; } = null!;
    public string AddressLine2 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;


    private Address()
    {
    }

    public Address(string addressLine1, string addressLine2, string city, string state, string zipCode)
    {
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        ZipCode = zipCode;
    }


    public override string ToString()
    {
        return $"{AddressLine1}, {AddressLine2}, {City}, {State}, {ZipCode}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine1;
        yield return AddressLine2;
        yield return City;
        yield return State;
        yield return ZipCode;
    }
}