using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    private ApplicationUser() // ef core
    {
    }

    public ApplicationUser(User user)
    {
        User = user;
        UserName = user.Email;
        Email = user.Email;
    }
}