using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Users.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Users.Queries;

public record GetDetailsForUserQuery : IRequest<ErrorOr<UserDto>>;

public class GetDetailsForUserHandler : IRequestHandler<GetDetailsForUserQuery, ErrorOr<UserDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetDetailsForUserHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<UserDto>> Handle(GetDetailsForUserQuery request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var user = await _dbContext.DomainUsers
            .Where(u => u.Id == userId)
            .ProjectToType<UserDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Error.Unexpected(
                code: "User.Null",
                description: "Unexpected User is null. This should never happen.");
        }

        return user;
    }
}

