using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Authentication;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // TODO: request validation
        var user = new User(Guid.NewGuid(), command.FirstName, command.LastName, command.Email, command.Address);
        
        var result = await _userService.RegisterUserAsync(user, command.Password);
        if (result.IsError)
        {
            return result.Errors;
        }
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthenticationResult(token);
    }
}
