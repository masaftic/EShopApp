using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Users.DTOs;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Users.Queries;

public record LoginWithRefreshTokenQuery(
    string RefreshToken) : IRequest<ErrorOr<AuthenticationResponse>>;


public class LoginWithRefreshTokenHandler : IRequestHandler<LoginWithRefreshTokenQuery, ErrorOr<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;

    public LoginWithRefreshTokenHandler(IIdentityService identityService, IApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<AuthenticationResponse>> Handle(LoginWithRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken, cancellationToken);
        
        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            return Error.Unauthorized(
                code: "InvalidRefreshToken",
                description: "Invalid refresh token");
        
        var user = await _identityService.GetUserByIdAsync(refreshToken.UserId);
        if (user.IsError)
            return user.Errors;

        var authResult = await _identityService.SignInAsync(user.Value);
        if (authResult.IsError)
            return authResult.Errors;
        
        var authToken = authResult.Value;
        
        refreshToken.Update(authToken.RefreshToken, DateTime.UtcNow.AddDays(7));
        _dbContext.RefreshTokens.Update(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResponse(
            AccessToken: authToken.AccessToken,
            RefreshToken: authToken.RefreshToken);
    }
}
