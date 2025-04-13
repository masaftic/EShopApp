using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Wishlists.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Wishlists.Queries;

public record GetWishlistQuery() : IRequest<ErrorOr<WishlistDto>>;

public class GetWishlistHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRequestHandler<GetWishlistQuery, ErrorOr<WishlistDto>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ErrorOr<WishlistDto>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var wishlist = await _dbContext.Wishlists
            .Include(w => w.WishlistItems)
            .Where(w => w.UserId == userId)
            .ProjectToType<WishlistDto>()
            .SingleAsync(cancellationToken);

        var wishlistDto = wishlist.Adapt<WishlistDto>();
        return wishlistDto;
    }
}