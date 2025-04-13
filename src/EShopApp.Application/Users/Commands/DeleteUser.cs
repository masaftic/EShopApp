using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using MediatR;

namespace EShopApp.Application.Users.Commands;

public record DeleteUserCommand(int UserId) : IRequest<ErrorOr<Deleted>>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteUserCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.DomainUsers.FindAsync([request.UserId], cancellationToken);

        if (user is null)
        {
            return DomainErrors.User.NotFound;
        }

        _dbContext.DomainUsers.Remove(user);

        var refreshTokens = _dbContext.RefreshTokens.Where(r => r.UserId == request.UserId);

        _dbContext.RefreshTokens.RemoveRange(refreshTokens);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}