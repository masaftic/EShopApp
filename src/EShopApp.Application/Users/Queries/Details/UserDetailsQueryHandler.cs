using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Queries.Details;

public class UserDetailsQueryHandler : IRequestHandler<UserDetailsQuery, Result<User>>
{
    private readonly IUserService _userService;

    public UserDetailsQueryHandler(IUserService userService)
    {
        _userService = userService;
    }


    public async Task<Result<User>> Handle(UserDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByIdAsync(request.Id);
        return result;
    }
}
