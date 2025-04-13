using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Application.Users.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    // public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedOnUtc { get; set; }
    public AddressDto? Address { get; set; }
}