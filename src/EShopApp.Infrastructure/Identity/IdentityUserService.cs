using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using FluentResults;
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

    public async Task<Result<User>> GetUserByEmailAsync(string email)
    {
        var applicationUser = await _userManager.FindByEmailAsync(email);
        if (applicationUser == null)
            return Result.Fail($"email '{email}' not found");
            
        var user = new User(applicationUser.Id, applicationUser.FirstName, applicationUser.LastName, applicationUser.Email!, applicationUser.Address);
        return Result.Ok(user);
    }

    public async Task<Result> RegisterUserAsync(User user, string password)
    {
        var applicationUser = new ApplicationUser
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Email,
            Email = user.Email,
            Address = user.Address
        };

        var result = await _userManager.CreateAsync(applicationUser, password);

        if (result.Succeeded)
        {
            return Result.Ok();
        }
        
        return Result.Fail(result.Errors.Select(e => e.Description).ToList());
    }


    public async Task<Result<User>> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return Result.Fail($"User with id '{userId}' was not found");

        return new User(user.Id, user.FirstName, user.LastName, user.Email, user.Address);
    }
    //
    // public async Task<bool> AssignRoleAsync(Guid userId, string role)
    // {
    //     var user = await _userManager.FindByIdAsync(userId.ToString());
    //     if (user == null) return false;
    //
    //     if (!await _roleManager.RoleExistsAsync(role))
    //     {
    //         await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
    //     }
    //
    //     var result = await _userManager.AddToRoleAsync(user, role);
    //     return result.Succeeded;
    // }
    //
    // public async Task<bool> ValidatePasswordAsync(Guid userId, string password)
    // {
    //     var user = await _userManager.FindByIdAsync(userId.ToString());
    //     if (user == null) return false;
    //
    //     return await _userManager.CheckPasswordAsync(user, password);
    // }
}