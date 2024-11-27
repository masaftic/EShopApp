using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Authentication;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResult>>
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User(Guid.NewGuid(), request.FirstName, request.LastName, request.Email, request.Address);
        
        var result = await _userService.RegisterUserAsync(user, request.Password);
        if (result.IsSuccess)
        {
            var token = _jwtTokenGenerator.GenerateToken(user);
            return Result.Ok(new AuthenticationResult(token));
        }

        return Result.Fail(result.Errors);
    }
}
