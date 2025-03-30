using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Wishlists.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace EShopApp.Application.Wishlists.Commands;

public record AddItemToWishlistCommand(int ProductId) : IRequest<ErrorOr<Created>>;

public class AddItemToWishlistHandler : IRequestHandler<AddItemToWishlistCommand, ErrorOr<Created>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AddItemToWishlistHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Created>> Handle(AddItemToWishlistCommand request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var wishlist = await _dbContext.Wishlists
            .Include(w => w.WishlistItems)
            .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

        if (wishlist is null)
        {
            wishlist = new Wishlist(userId);
            await _dbContext.Wishlists.AddAsync(wishlist, cancellationToken);
        }

        if (await _dbContext.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken) == false)
            return DomainErrors.Product.NotFound;

        var result = wishlist.AddItem(request.ProductId);
        if (result.IsError)
            return result.Errors;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Created;
    }
}

