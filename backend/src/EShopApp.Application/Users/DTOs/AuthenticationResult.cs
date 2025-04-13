namespace EShopApp.Application.Users.DTOs;

public record AuthenticationResult
(
    string AccessToken,
    string RefreshToken,
    int UserId
);