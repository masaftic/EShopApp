using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.UpdateCart;


public record UpdateCartCommand(
    int ProductId, 
    // Quantity to be set not incremented
    int Quantity) : IRequest<ErrorOr<CartItemDto>>;

public class AddToCartCommandHandler : IRequestHandler<UpdateCartCommand, ErrorOr<CartItemDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public AddToCartCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<CartItemDto>> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);

        var userCart = await _dbContext.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.CartItems)
            .SingleAsync(cancellationToken);

        if (userCart is null)
            return Error.NotFound(description: "Cart not found");

        var product = await _dbContext.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);

        if (product is null)
            return Errors.Product.NotFound;

        var cartItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
        if (cartItem is null)
            return Error.NotFound(description: $"Cart item with product id {request.ProductId} not found");

        cartItem.SetQuantity(request.Quantity);
        cartItem.UpdatePrice(product.Price);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return cartItem.Adapt<CartItemDto>();
    }
}