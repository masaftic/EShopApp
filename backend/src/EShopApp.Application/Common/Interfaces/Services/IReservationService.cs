using ErrorOr;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Interfaces.Services;

public interface IReservationService
{
    Task<ErrorOr<Success>> CreateReservationAsync(int userId, string paymentIntentId, List<CartItem> cartItems, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> ReleaseReservationAsync(Reservation reservation, CancellationToken cancellationToken);
    Task<ErrorOr<Success>> FinalizeReservationAsync(Reservation reservation, CancellationToken cancellationToken);
    Task<ErrorOr<Reservation>> ExtendExistingReservationAsync(int userId, CancellationToken cancellationToken);
}
