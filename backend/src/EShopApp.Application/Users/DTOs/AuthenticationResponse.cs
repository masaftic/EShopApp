namespace EShopApp.Application.Users.DTOs;

public record AuthenticationResponse(
    string AccessToken,
    string RefreshToken);