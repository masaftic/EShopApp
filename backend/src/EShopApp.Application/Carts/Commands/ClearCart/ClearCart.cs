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
    private readonly IReservationService _reservationService;

    public ClearCartCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IReservationService reservationService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _reservationService = reservationService;
    }

    public async Task<ErrorOr<Success>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var userCart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .SingleAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        var reservation = await _dbContext.Reservations
            .Include(r => r.ReservationItems)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active, cancellationToken: cancellationToken);

        if (reservation != null)
        {
            await _reservationService.ReleaseReservationAsync(reservation, cancellationToken);
        }

        userCart.ClearCart();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}