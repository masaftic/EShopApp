using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }

    public ICollection<Order> Orders { get; set; }
    public Cart Cart { get; set; }
    public ICollection<Domain.Entities.Payment> Payments { get; set; }

    public User ToDomainUser()
    {
        var user = new User(this.FirstName, this.LastName, this.Email!, this.Address)
        {
            Id = this.Id
        };

        return user;
    }

    public static ApplicationUser FromUser(User user)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Email,
            Email = user.Email,
            Address = user.Address
        };
        return applicationUser;
    }
}