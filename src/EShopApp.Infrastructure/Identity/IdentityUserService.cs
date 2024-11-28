using System.Data.SqlTypes;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using EShopApp.Domain.Errors;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Identity;

public class IdentityUserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public IdentityUserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<User>> GetUserByEmailAsync(string email)
    {
        var applicationUser = await _userManager.FindByEmailAsync(email);
        if (applicationUser == null)
            return Errors.User.InvalidCredentials;
            
        var user = new User(applicationUser.Id, applicationUser.FirstName, applicationUser.LastName, applicationUser.Email!, applicationUser.Address);
        return user;
    }

    public async Task<ErrorOr<bool>> RegisterUserAsync(User user, string password)
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
        
        var result = await _userManager.CreateAsync(applicationUser, password);
        if (result.Succeeded)
            return true;
        
        return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
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
        
        return await _userManager.CheckPasswordAsync(applicationUser, password);
    }

    public async Task<ErrorOr<User>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Error.NotFound("User.IdNotFound", $"User with id '{userId}' was not found");

        return new User(user.Id, user.FirstName, user.LastName, user.Email, user.Address);
    }
}