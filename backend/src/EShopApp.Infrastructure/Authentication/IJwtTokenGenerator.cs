using EShopApp.Application.Common.DTOs;
using EShopApp.Infrastructure.Data.Identity;

namespace EShopApp.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
    string GenerateRefreshToken();
}
