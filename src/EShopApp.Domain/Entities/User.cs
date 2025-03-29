using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Address? Address { get; private set; }
    public ICollection<Order> Orders { get; set; } = [];
    public Cart? Cart { get; set; }
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];


    private User() // ef core
    {
    }

    public User(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}