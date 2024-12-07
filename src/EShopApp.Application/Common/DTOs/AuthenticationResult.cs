namespace EShopApp.Application.Common.DTOs;

public class AuthenticationResult(string token, int expiresIn)
{ 
    public string Token { get; } = token;
    public int ExpiresIn { get; } = expiresIn; // Seconds
}