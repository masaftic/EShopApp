using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.Checkout;

public record CheckoutCommand() : IRequest<ErrorOr<CartDto>>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, ErrorOr<CartDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }


    public async Task<ErrorOr<CartDto>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var cart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (cart is null)
            return Error.NotFound(description: "Cart not found");

        if (cart.CartItems.Count == 0)
            return Error.Conflict(description: "Cannot checkout on an empty cart");

        // Update price to time of checkout
        foreach (var cartItem in cart.CartItems)
        {
            cartItem.UpdatePrice(cartItem.Product.Price);
        }

        var inventoryProducts = await _dbContext.Inventories
            .Where(i => cart.CartItems.Select(ci => ci.ProductId).Contains(i.ProductId))
            .ToListAsync(cancellationToken);

        // Validate stock
        foreach (var cartItem in cart.CartItems)
        {
            var inventory = inventoryProducts.First(i => i.ProductId == cartItem.ProductId);
            if (inventory.AvailableStock < cartItem.Quantity)
                return Error.Conflict(description: $"Insufficient stock for product {cartItem.ProductId}");
        }


        var sessionExpiryDate = DateTime.UtcNow.Add(Cart.DefaultSessionExpiryDuration);
        cart.SetExpiryDate(sessionExpiryDate);

        var reservation = new Reservation
        {
            UserId = userId,
            Status = ReservationStatus.Active,
            ExpirationDate = sessionExpiryDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ReservationItems = cart.CartItems.Select(ci => new ReservationItem
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity
            }).ToList()
        };

        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);

        // TODO: Domain events

        // Reserve products
        foreach (var cartItem in cart.CartItems)
        {
            var inventory = inventoryProducts.First(i => i.ProductId == cartItem.ProductId);
            inventory.Reserve(cartItem.Quantity);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return cart.Adapt<CartDto>();
    }
}