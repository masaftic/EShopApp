using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public int UserId { get; set; }
    public User User { get; set; }

    public static ApplicationUser FromUser(User user)
    {
        var applicationUser = new ApplicationUser
        {
            UserId = user.Id,
            User = user,
            UserName = user.Email,
            Email = user.Email
        };

        return applicationUser;
    }
}