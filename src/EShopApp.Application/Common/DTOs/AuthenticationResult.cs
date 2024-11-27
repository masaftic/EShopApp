namespace EShopApp.Application.Common.DTOs;

public class AuthenticationResult
{ 
    public string Token { get; set; }

    public AuthenticationResult(string token)
    {
        Token = token;
    }
}