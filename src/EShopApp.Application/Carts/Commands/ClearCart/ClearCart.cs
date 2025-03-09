using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.ClearCart;

public record ClearCartCommand : IRequest<ErrorOr<Success>>;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ClearCartCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var userCart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .SingleOrDefaultAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (userCart == null)
            return Error.NotFound(description: "Cart not found");

        var reservation = await _dbContext.Reservations
            .Include(r => r.ReservationItems)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active, cancellationToken: cancellationToken);

        if (reservation != null)
        {
            // Release inventory reservations
            foreach (var reservationItem in reservation.ReservationItems)
            {
                var inventory = await _dbContext.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == reservationItem.ProductId, cancellationToken: cancellationToken);

                inventory?.Release(reservationItem.Quantity);
            }

            _dbContext.Reservations.Remove(reservation);
        }

        userCart.CartItems.Clear();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}