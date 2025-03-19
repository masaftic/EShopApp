using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.AddToCart;

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
            .SingleAsync(cancellationToken);

        var product = await _dbContext.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);

        if (product == null)
            return Errors.Product.NotFound;

        if (product.Inventory is null || product.Inventory.AvailableStock < request.Quantity)
            return Error.Conflict(description: "Out of stock");

        var cartItem = userCart.AddToCart(product.Id, request.Quantity, product.Price);
        if (cartItem.Quantity > product.Inventory.AvailableStock)
            return Error.Conflict(description: "Not enough stock");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return cartItem.Adapt<CartItemDto>();
    }
}