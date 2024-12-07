using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Queries.Details;

public class UserDetailsQueryHandler : IRequestHandler<UserDetailsQuery, ErrorOr<User>>
{
    private readonly IIdentityService _identityService;

    public UserDetailsQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }


    public async Task<ErrorOr<User>> Handle(UserDetailsQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine(request.Id.ToString());
        var result = await _identityService.GetUserByIdAsync(request.Id);
        return result;
    }
}
