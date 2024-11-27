using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public Address Address { get; private set; }
    
    public User(Guid id, string firstName, string lastName, string email, Address address)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Address = address;
    }
}
