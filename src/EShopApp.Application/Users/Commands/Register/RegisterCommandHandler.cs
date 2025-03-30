using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;
using EShopApp.Application.Users.DTOs;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;

    public RegisterCommandHandler(IIdentityService identityService, IApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<AuthenticationResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var user = new User(command.FirstName, command.LastName, command.Email);

        var authResult = await _identityService.SignUpAsync(user, command.Password);
        if (authResult.IsError)
            return authResult.Errors;

        var refreshToken = new RefreshToken(
            authResult.Value.AccessToken,
            authResult.Value.UserId,
            DateTime.UtcNow.AddDays(7));

        var cart = new Cart(user.Id);

        await _dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _dbContext.Carts.AddAsync(cart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResponse(
            authResult.Value.AccessToken,
            refreshToken.Token);
    }
}
