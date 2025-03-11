using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EShopApp.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(string token, int expiresIn)> GenerateTokenAsync(ApplicationUser user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256Signature
        );

        var claims = await GetUserClaimsAsync(user);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwtToken), _jwtSettings.ExpiryMinutes * 60);
    }

    private async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.GivenName, user.User.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.User.LastName),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var restOfClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(restOfClaims);
        
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }
}
