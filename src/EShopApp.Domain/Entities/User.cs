using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public Address Address { get; private set; }

    private User()
    {
    }

    public User(string firstName, string lastName, string email, Address address)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Address = address;
    }
}