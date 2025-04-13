using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Users.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Users.Queries;

public record GetAllUsersQuery : IRequest<ErrorOr<List<UserDto>>>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<List<UserDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllUsersHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _dbContext.DomainUsers.ProjectToType<UserDto>().ToListAsync(cancellationToken);
        return users;
    }
}

