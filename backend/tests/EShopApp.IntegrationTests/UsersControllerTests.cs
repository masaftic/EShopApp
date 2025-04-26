using System.Net.Http.Headers;
using System.Net.Http.Json;
using EShopApp.Application.Users.Commands.Login;
using EShopApp.Application.Users.Commands.Register;
using EShopApp.Application.Users.DTOs;
using EShopApp.Application.Users.Queries.Details;

namespace EShopApp.IntegrationTests;

public class UsersControllerTests
{
    [Fact]
    public async Task RegisterRequest_ShouldReturnAuthResult_WhenSuccessful()
    {
        // Arrange
        var factory = new EShopAppWebApplicationFactory();
        var registerRequest = new RegisterCommand("fname", "lname", "test@gmail.com", "Password123!");
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/users/register", registerRequest);

        // Assert
        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.AccessToken);
        Assert.NotEmpty(authResponse.RefreshToken);
    }

    [Fact]
    public async Task LoginRequest_ShouldReturnAuthResult_WhenCredentialsAreValid()
    {
        // Arrange
        var factory = new EShopAppWebApplicationFactory();
        var client = factory.CreateClient();
        var registerRequest = new RegisterCommand("testlogin", "user", "testlogin@gmail.com", "Password123!");
        await client.PostAsJsonAsync("/api/users/register", registerRequest); // Register the user first

        var loginRequest = new LoginCommand("testlogin@gmail.com", "Password123!");

        // Act
        var response = await client.PostAsJsonAsync("/api/users/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        Assert.NotNull(authResponse);
        Assert.NotEmpty(authResponse.AccessToken);
        Assert.NotEmpty(authResponse.RefreshToken);
    }

    [Fact]
    public async Task GetUserDetails_ShouldReturnUserDetails_WhenAuthenticated()
    {
        // Arrange
        var factory = new EShopAppWebApplicationFactory();
        var client = factory.CreateClient();
        var registerRequest = new RegisterCommand("details", "user", "details@gmail.com", "Password123!");
        await client.PostAsJsonAsync("/api/users/register", registerRequest); // Register the user

        var loginRequest = new LoginCommand("details@gmail.com", "Password123!");
        var loginResponse = await client.PostAsJsonAsync("/api/users/login", loginRequest); // Login to get token
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        Assert.NotNull(authResponse);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        response.EnsureSuccessStatusCode();
        var userDetails = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(userDetails);
        Assert.Equal("details@gmail.com", userDetails.Email);
        Assert.Equal("details", userDetails.FirstName);
        Assert.Equal("user", userDetails.LastName);
    }
}
