using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Queries.Details;

public class UserDetailsQueryHandler : IRequestHandler<UserDetailsQuery, ErrorOr<User>>
{
    private readonly IUserService _userService;

    public UserDetailsQueryHandler(IUserService userService)
    {
        _userService = userService;
    }


    public async Task<ErrorOr<User>> Handle(UserDetailsQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine(request.Id.ToString());
        var result = await _userService.GetUserByIdAsync(request.Id);
        return result;
    }
}
