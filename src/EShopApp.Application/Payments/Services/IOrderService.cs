using EShopApp.Domain.Entities;

namespace EShopApp.Application.Payments.Services;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(Reservation reservation, Payment payment, CancellationToken cancellationToken);
}