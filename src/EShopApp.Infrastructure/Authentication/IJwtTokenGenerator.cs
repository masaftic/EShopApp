using EShopApp.Infrastructure.Data.Identity;

namespace EShopApp.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user);
}
