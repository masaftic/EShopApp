using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Users.Commands.Register;
using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using ErrorOr;

namespace EShopApp.Tests.ApplicationTests.Users.Commands;

public class RegisterTests
{
    public RegisterTests()
    {
    }

    // [Fact]
    // public async Task Handle_ValidCommand_ReturnsAuthenticationResult()
    // {
    //     // Arrange
    //     var command = new RegisterCommand
    //     (
    //         FirstName: "John",
    //         LastName: "Doe",
    //         Email: "john.doe@example.com",
    //         Address: new Address("123", "Main", "St"),
    //         Password: "SecurePassword123!"
    //     );
    //
    //     var sampleAuthResult = new AuthenticationResult("SampleJWTToken", 3000);
    //     var identityServiceMock = new Mock<IIdentityService>();
    //
    //     identityServiceMock
    //         .Setup(s => s.SignUpAsync(It.IsAny<User>(), It.IsAny<string>()))
    //         .ReturnsAsync(sampleAuthResult);
    //     
    //     var handler = new RegisterCommandHandler(identityServiceMock.Object);
    //     
    //     // Act
    //     var result = await handler.Handle(command, default);
    //
    //     
    //     // Assert
    //     Assert.True(!result.IsError, "The registration should succeed.");
    //     Assert.Equal(sampleAuthResult.Token, result.Value.Token);
    //     identityServiceMock.Verify(
    //         s => s.SignUpAsync(It.Is<User>(u =>
    //                 u.Email == command.Email &&
    //                 u.FirstName == command.FirstName &&
    //                 u.LastName == command.LastName &&
    //                 u.Address == command.Address),
    //             command.Password),
    //         Times.Once);
    // }
    //
    //
    // [Fact]
    // public async Task Handle_SignUpFails_ReturnsError()
    // {
    //     // Arrange
    //     var identityServiceMock = new Mock<IIdentityService>();
    //     var command = new RegisterCommand
    //     (
    //         FirstName: "John",
    //         LastName: "Doe",
    //         Email: "john.doe@example.com",
    //         Address: new Address("123", "Main", "St"),
    //         Password: "badpassword"
    //     );
    //
    //     var error = Error.Validation("SignUpFailed", "Unable to register user.");
    //     identityServiceMock
    //         .Setup(s => s.SignUpAsync(It.IsAny<User>(), It.IsAny<string>()))
    //         .ReturnsAsync(error);
    //
    //     var handler = new RegisterCommandHandler(identityServiceMock.Object);
    //
    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);
    //
    //     // Assert
    //     Assert.True(result.IsError, "The registration should fail.");
    //     Assert.Contains(result.Errors, e => e.Code == "SignUpFailed");
    //     identityServiceMock.Verify(
    //         s => s.SignUpAsync(It.IsAny<User>(), It.IsAny<string>()),
    //         Times.Once);
    // }
}