using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using EShopApp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace EShopApp.Infrastructure.Data.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<User>> GetUserByEmailAsync(string email)
    {
        var applicationUser = await _userManager.FindByEmailAsync(email);
        if (applicationUser is null)
            return Errors.User.InvalidCredentials;

        return applicationUser.User;
    }

    public async Task<ErrorOr<AuthenticationResult>> SignUpAsync(User user, string password)
    {
        var applicationUser = ApplicationUser.FromUser(user);

        var result = await _userManager.CreateAsync(applicationUser, password);
        if (!result.Succeeded) 
            return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        
        var (token, expiresIn) = await _jwtTokenGenerator.GenerateTokenAsync(applicationUser);
        return new AuthenticationResult(token, expiresIn);
    }

    public async Task<ErrorOr<AuthenticationResult>> SignInAsync(string email, string password)
    {
        var applicationUser = await _userManager.FindByEmailAsync(email);
        if (applicationUser is null)
            return Errors.User.InvalidCredentials;

        var result = await _userManager.CheckPasswordAsync(applicationUser, password);
        if (!result)
            return Errors.User.InvalidCredentials;

        var (token, expiresIn) = await _jwtTokenGenerator.GenerateTokenAsync(applicationUser);
        return new AuthenticationResult(token, expiresIn);
    }

    public async Task<ErrorOr<User>> GetUserByIdAsync(int userId)
    {
        var applicationUser = await _userManager.FindByIdAsync(userId.ToString());
        if (applicationUser is null)
            return Errors.User.NotFound;

        return applicationUser.User;
    }
}