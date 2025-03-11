using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.Commands.CreatePayment;
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
        var cart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (cart is null)
            return Error.NotFound(description: "Cart not found");

        if (cart.CartItems.Count == 0)
            return Error.Conflict(description: "Cannot checkout on an empty cart");

        if (cart.SessionExpiryDate is not null && cart.SessionExpiryDate > DateTime.UtcNow)
        {
            var reservation = await _dbContext.Reservations
                           .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active, cancellationToken: cancellationToken);


            if (reservation != null)
            {
                // Extend the reservation expiry time
                reservation.ExpirationDate = DateTime.UtcNow.AddMinutes(10);
                await _dbContext.SaveChangesAsync(cancellationToken);

                Console.WriteLine($"Retrieved PaymentIntentId: {reservation.PaymentIntentId}");
                return await _paymentService.GetPaymentIntentAsync(reservation.PaymentIntentId);
            }
        }

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
                return Error.Conflict(description: $"No inventory found for product {cartItem.ProductId}");
            }

            if (inventory.AvailableStock < cartItem.Quantity)
            {
                return Error.Conflict(description: $"Insufficient stock for product {cartItem.ProductId}");
            }
        }

        using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                var sessionExpiryDate = DateTime.UtcNow.Add(Cart.DefaultSessionExpiryDuration);
                cart.SetExpiryDate(sessionExpiryDate);

                var reservation = new Reservation
                {
                    UserId = userId,
                    // SessionId = Guid.NewGuid().ToString(),
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

                    var inventoryTransaction = new InventoryTransaction(inventory.Id, cartItem.Quantity,
                        InventoryTransactionType.Reserve, DateTime.UtcNow, "Checkout Reservation");
                    inventoryTransactions.Add(inventoryTransaction);
                }

                await _dbContext.InventoryTransactions.AddRangeAsync(inventoryTransactions, cancellationToken);

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

                var paymentIntentResult = await _paymentService.CreatePaymentIntentAsync(options);

                if (paymentIntentResult.IsError)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return paymentIntentResult;
                }

                reservation.PaymentIntentId = paymentIntentResult.Value.PaymentIntentId;

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
    }
}