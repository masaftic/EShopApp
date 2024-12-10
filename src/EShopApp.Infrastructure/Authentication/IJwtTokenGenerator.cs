using EShopApp.Infrastructure.Data.Identity;

namespace EShopApp.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    Task<(string token, int expiresIn)> GenerateTokenAsync(ApplicationUser user);
}
