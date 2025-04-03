using EShopApp.Application.Common.Interfaces.Persistence;
using MediatR;
using ErrorOr;
using EShopApp.Domain.Entities;
using EShopApp.Application.Users.DTOs;

namespace EShopApp.Application.Users.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResponse>>;


public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;

    public LoginCommandHandler(IIdentityService identityService, IApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginResult = await _identityService.SignInAsync(request.Email, request.Password);
        if (loginResult.IsError)
            return loginResult.Errors;

        var authResult = loginResult.Value;
        var refreshToken = new RefreshToken(
            authResult.RefreshToken, 
            authResult.UserId, 
            DateTime.UtcNow.AddDays(7));

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResponse(authResult.AccessToken, refreshToken.Token);
    }
}