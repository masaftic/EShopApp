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

        var productsInventories = await _dbContext.Inventories
            .Where(i => cart.CartItems.Select(ci => ci.ProductId).Contains(i.ProductId))
            .ToDictionaryAsync(key => key.ProductId, value => value, cancellationToken: cancellationToken);

        // Validate stock
        foreach (var cartItem in cart.CartItems)
        {
            if (!productsInventories.TryGetValue(cartItem.ProductId, out var inventory))
            {
                return Error.Conflict(description: $"Insufficient stock for product {cartItem.ProductId}");
            }
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

        var inventoryTransactions = new List<InventoryTransaction>();

        // Reserve products & write the inventory transactions
        foreach (var cartItem in cart.CartItems)
        {
            var inventory = productsInventories[cartItem.ProductId];
            inventory.Reserve(cartItem.Quantity);

            var transaction = new InventoryTransaction(inventory.Id, cartItem.Quantity,
                InventoryTransactionType.Reserve, DateTime.UtcNow, "Checkout Reservation");
            inventoryTransactions.Add(transaction);
        }

        await _dbContext.InventoryTransactions.AddRangeAsync(inventoryTransactions, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return cart.Adapt<CartDto>();
    }
}