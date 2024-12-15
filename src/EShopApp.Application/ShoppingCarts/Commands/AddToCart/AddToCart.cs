using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.ShoppingCarts.Commands.AddToCart;

public record AddToCartCommand(int ProductId, int Quantity) : IRequest<ErrorOr<CartItemDto>>;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, ErrorOr<CartItemDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public AddToCartCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<CartItemDto>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);

        var userCart = await _dbContext.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.CartItems)
            .SingleOrDefaultAsync(cancellationToken);

        if (userCart == null)
        {
            userCart = new Cart(userId);
            _dbContext.Carts.Add(userCart);
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId,
                cancellationToken: cancellationToken);

        if (product == null)
        {
            return Errors.Product.NotFound;
        }

        var cartItem = userCart.AddToCart(request.ProductId, request.Quantity, product.Price);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return cartItem.ToDto();
    }
}