using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; }
}