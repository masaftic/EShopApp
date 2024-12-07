using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }


    public User ToDomainUser()
    {
        return new User(this.Id, this.FirstName, this.LastName, this.Email, this.Address); 
    }

    public static ApplicationUser FromUser(User user)
    {
        var applicationUser = new ApplicationUser
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Email,
            Email = user.Email,
            Address = user.Address
        };
        return applicationUser;
    }
}