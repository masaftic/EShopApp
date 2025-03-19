using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.Checkout;

public record CheckoutCommand() : IRequest<ErrorOr<PaymentIntentResult>>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, ErrorOr<PaymentIntentResult>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPaymentService _paymentService;

    public CheckoutCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IPaymentService paymentService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _paymentService = paymentService;
    }

    public async Task<ErrorOr<PaymentIntentResult>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var cart = await GetUserCartWithProductsAndInventoriesAsync(userId, cancellationToken);

        if (cart.CartItems.Count == 0)
            return Error.Conflict(description: "Cannot checkout on an empty cart");

        // Check for existing session and reservation
        if (cart.SessionExpiryDate is not null && cart.SessionExpiryDate > DateTime.UtcNow)
        {
            var existingPaymentResult = await GetExistingPaymentAsync(userId, cancellationToken);
            if (!existingPaymentResult.IsError)
                return existingPaymentResult;
        }

        // Update cart item prices
        UpdateCartItemPrices(cart);

        // Validate inventory
        var productsInventories = GetProductsInventoriesAsync(cart, cancellationToken);
        var inventoryValidation = ValidateInventory(cart, productsInventories);
        if (inventoryValidation.IsError)
            return inventoryValidation.Errors;

        // Process checkout with transaction
        return await ProcessCheckoutTransactionAsync(cart, userId, productsInventories, cancellationToken);
    }

    private async Task<Cart> GetUserCartWithProductsAndInventoriesAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p!.Inventory)
            .SingleAsync(c => c.UserId == userId, cancellationToken: cancellationToken);
    }

    private async Task<ErrorOr<PaymentIntentResult>> GetExistingPaymentAsync(int userId, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active,
                                cancellationToken: cancellationToken);

        if (reservation != null)
        {
            // Extend the reservation expiry time
            reservation.ExpirationDate = DateTime.UtcNow.AddMinutes(10);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return await _paymentService.GetPaymentIntentAsync(reservation.PaymentIntentId);
        }

        return default;
    }

    private void UpdateCartItemPrices(Cart cart)
    {
        foreach (var cartItem in cart.CartItems)
        {
            cartItem.UpdatePrice(cartItem.Product!.Price);
        }
    }

    private Dictionary<int, Inventory> GetProductsInventoriesAsync(Cart cart, CancellationToken cancellationToken)
    {
        return cart.CartItems.ToDictionary(key => key.ProductId, value => value.Product!.Inventory);
    }

    private ErrorOr<Success> ValidateInventory(Cart cart, Dictionary<int, Inventory> productsInventories)
    {
        foreach (var cartItem in cart.CartItems)
        {
            if (!productsInventories.TryGetValue(cartItem.ProductId, out var inventory))
            {
                return Error.Conflict(description: $"No inventory found for product {cartItem.ProductId}");
            }

            if (inventory.AvailableStock < cartItem.Quantity)
            {
                return Error.Conflict(description: $"Insufficient stock for product {cartItem.ProductId}");
            }
        }

        return Result.Success;
    }

    private async Task<ErrorOr<PaymentIntentResult>> ProcessCheckoutTransactionAsync(
        Cart cart, int userId, Dictionary<int, Inventory> productsInventories, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var sessionExpiryDate = DateTime.UtcNow.Add(Cart.DefaultSessionExpiryDuration);
            cart.SetExpiryDate(sessionExpiryDate);

            var inventoryTransactions = CreateInventoryTransactions(cart, productsInventories);
            await _dbContext.InventoryTransactions.AddRangeAsync(inventoryTransactions, cancellationToken);

            var paymentIntentResult = await CreatePaymentIntentAsync(cart, userId);
            if (paymentIntentResult.IsError)
            {
                await transaction.RollbackAsync(cancellationToken);
                return paymentIntentResult;
            }

            var reservation = CreateReservation(userId, paymentIntentResult.Value.PaymentIntentId, cart);

            await _dbContext.Reservations.AddAsync(reservation, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return paymentIntentResult;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Error.Failure(description: ex.Message);
        }
    }

    private Reservation CreateReservation(int userId, string paymentIntentId, Cart cart)
    {
        var reservation = new Reservation(userId, paymentIntentId);
        var items = cart.CartItems.Select(ci => (ci.ProductId, ci.Quantity, ci.UnitPrice));
        reservation.AddItems(items);
        return reservation;
    }

    private List<InventoryTransaction> CreateInventoryTransactions(Cart cart, Dictionary<int, Inventory> productsInventories)
    {
        var inventoryTransactions = new List<InventoryTransaction>();

        foreach (var cartItem in cart.CartItems)
        {
            var inventory = productsInventories[cartItem.ProductId];
            inventory.Reserve(cartItem.Quantity);

            var inventoryTransaction = new InventoryTransaction(inventory.Id, cartItem.Quantity,
                InventoryTransactionType.Reserve, DateTime.UtcNow, "Checkout Reservation");
            inventoryTransactions.Add(inventoryTransaction);
        }

        return inventoryTransactions;
    }

    private async Task<ErrorOr<PaymentIntentResult>> CreatePaymentIntentAsync(Cart cart, int userId)
    {
        var options = new PaymentIntentOptionsDto
        {
            Amount = (long)(cart.TotalPrice * 100), // in smallest currency unit (e.g., cents for USD)
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                { "cart_id", cart.Id.ToString() },
                { "user_id", userId.ToString() }
            }
        };

        return await _paymentService.CreatePaymentIntentAsync(options);
    }
}