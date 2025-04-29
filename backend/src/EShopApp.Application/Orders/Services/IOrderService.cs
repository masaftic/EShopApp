using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Application.Payments.Services;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(int userId, Reservation reservation, Payment payment, Address shippingAddress, CancellationToken cancellationToken);
}