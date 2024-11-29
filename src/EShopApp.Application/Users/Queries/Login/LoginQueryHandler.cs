using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Authentication;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using MediatR;
using ErrorOr;

namespace EShopApp.Application.Users.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IUserService _userService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginQueryHandler(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByEmailAsync(request.Email);
        if (result.IsError)
        {
            return Errors.User.InvalidCredentials;
        }

        if (!await _userService.CheckPasswordAsync(result.Value.Id, request.Password))
        {
            return Errors.User.InvalidCredentials;
        }
        
        var token = _jwtTokenGenerator.GenerateToken(result.Value);
        return new AuthenticationResult(token);
    }
}
