using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;

    public RegisterCommandHandler(IIdentityService identityService, IApplicationDbContext dbContext)
    {
        _identityService = identityService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var user = new User(command.FirstName, command.LastName, command.Email, command.Address);

        var result = await _identityService.SignUpAsync(user, command.Password);

        if (!result.IsError)
        {
            var cart = new Cart(user.Id);
            await _dbContext.Carts.AddAsync(cart, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
