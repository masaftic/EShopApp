using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Authentication;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthenticationResult>>
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginQueryHandler(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByEmailAsync(request.Email);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }
        
        var token = _jwtTokenGenerator.GenerateToken(result.Value);
        return new AuthenticationResult(token);
    }
}
