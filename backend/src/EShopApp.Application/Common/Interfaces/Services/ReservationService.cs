using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Inventories.Services;
using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Common.Interfaces.Services;

public class ReservationService : IReservationService
{
    private readonly IInventoryService _inventoryService;
    private readonly IApplicationDbContext _dbContext;

    public ReservationService(IInventoryService inventoryService, IApplicationDbContext dbContext)
    {
        _inventoryService = inventoryService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> CreateReservationAsync(int userId, string paymentIntentId, List<CartItem> cartItems, CancellationToken cancellationToken)
    {
        // Create reservation
        var reservation = new Reservation(userId, paymentIntentId);
        reservation.AddItems(cartItems.Select(ci => (ci.ProductId, ci.Quantity, ci.UnitPrice)).ToList());
        await _dbContext.Reservations.AddAsync(reservation, cancellationToken);

        // Adjust inventory
        return await _inventoryService.AdjustInventoryAsync(cartItems.Select(p => (p.ProductId, p.Quantity)).ToList(),
            "Reservation",
            InventoryTransactionType.Reserve,
            cancellationToken);
    }

    public async Task<ErrorOr<Reservation>> ExtendExistingReservationAsync(int userId, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active, cancellationToken);

        if (reservation != null)
        {
            // Extend the reservation expiry time
            reservation.ExpirationDate = DateTime.UtcNow.AddMinutes(10);
            return reservation;
        }

        return Error.NotFound("No active reservation found for the user.");
    }

    public Task<ErrorOr<Success>> FinalizeReservationAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        reservation.Status = ReservationStatus.Fulfilled;
        reservation.UpdatedAt = DateTime.UtcNow;
        var productQuantities = reservation.ReservationItems.Select(i => (i.ProductId, i.Quantity)).ToList();
        return _inventoryService.AdjustInventoryAsync(productQuantities,
            "Finalize Reservation",
            InventoryTransactionType.Outbound,
            cancellationToken);
    }

    public Task<ErrorOr<Success>> ReleaseReservationAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        reservation.Status = ReservationStatus.Cancelled;
        reservation.UpdatedAt = DateTime.UtcNow;
        var productQuantities = reservation.ReservationItems.Select(i => (i.ProductId, i.Quantity)).ToList();
        return _inventoryService.AdjustInventoryAsync(productQuantities,
            "Release Reservation",
            InventoryTransactionType.Release,
            cancellationToken);
    }
}
