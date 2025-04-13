using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Users.Commands;

public record RevokeTokenCommand(int UserId) : IRequest<ErrorOr<Deleted>>;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;

    public RevokeTokenCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var tokensToDelete = await _dbContext.RefreshTokens
            .Where(r => r.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        _dbContext.RefreshTokens.RemoveRange(tokensToDelete);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}