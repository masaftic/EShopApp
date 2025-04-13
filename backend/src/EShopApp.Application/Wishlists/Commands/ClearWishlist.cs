using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Wishlists.Commands;

public record ClearWishlistCommand() : IRequest<ErrorOr<Deleted>>;

public class ClearWishlistHandler : IRequestHandler<ClearWishlistCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ClearWishlistHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Deleted>> Handle(ClearWishlistCommand request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var wishlist = await _dbContext.Wishlists
            .Include(w => w.WishlistItems)
            .SingleAsync(w => w.UserId == userId, cancellationToken);

        wishlist.Clear();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
