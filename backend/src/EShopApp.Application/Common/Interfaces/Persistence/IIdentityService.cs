using EShopApp.Domain.Entities;
using ErrorOr;
using EShopApp.Application.Users.DTOs;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IIdentityService
{
    Task<ErrorOr<User>> GetUserByIdAsync(int userId);
    Task<ErrorOr<User>> GetUserByEmailAsync(string email);
    Task<ErrorOr<AuthenticationResult>> SignUpAsync(User user, string password);
    Task<ErrorOr<AuthenticationResult>> SignInAsync(User user);
    Task<ErrorOr<AuthenticationResult>> SignInAsync(string email, string password);


    // Task<string?> GetUserNameAsync(string userId);
    //
    // Task<bool> IsInRoleAsync(string userId, string role);
    //
    // Task<bool> AuthorizeAsync(string userId, string policyName);
    //
    // Task<Result> DeleteUserAsync(string userId);
}
