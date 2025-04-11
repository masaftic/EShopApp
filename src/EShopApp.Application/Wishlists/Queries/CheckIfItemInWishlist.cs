using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Wishlists.Queries;

public record CheckIfItemInWishlistQuery(int ProductId) : IRequest<ErrorOr<bool>>;

public class CheckIfItemInWishlistHandler : IRequestHandler<CheckIfItemInWishlistQuery, ErrorOr<bool>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CheckIfItemInWishlistHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<bool>> Handle(CheckIfItemInWishlistQuery request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var exists = await _dbContext.Wishlists
            .Include(w => w.WishlistItems)
            .AnyAsync(w => w.UserId == userId && 
                          w.WishlistItems.Any(i => i.ProductId == request.ProductId), 
                    cancellationToken);

        return exists;
    }
}
