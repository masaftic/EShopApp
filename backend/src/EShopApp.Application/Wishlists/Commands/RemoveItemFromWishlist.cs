using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Wishlists.Commands;

public record RemoveItemFromWishlistCommand(int ProductId) : IRequest<ErrorOr<Deleted>>;

public class RemoveItemFromWishlistHandler : IRequestHandler<RemoveItemFromWishlistCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public RemoveItemFromWishlistHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Deleted>> Handle(RemoveItemFromWishlistCommand request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var wishlist = await _dbContext.Wishlists
            .Include(w => w.WishlistItems)
            .SingleAsync(w => w.UserId == userId, cancellationToken);

        if (wishlist is null)
            return Error.NotFound("Wishlist.NotFound", "Wishlist not found");

        var result = wishlist.RemoveItem(request.ProductId);
        if (result.IsError)
            return result.Errors;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
