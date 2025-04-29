using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Inventories.Services;
using EShopApp.Application.Payments.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IReservationService _reservationService;

    public OrderService(IApplicationDbContext dbContext, IReservationService reservationService)
    {
        _dbContext = dbContext;
        _reservationService = reservationService;
    }

    public async Task<Order> PlaceOrderAsync(int userId, Reservation reservation, Payment payment, Address shippingAddress, CancellationToken cancellationToken)
    {
        var order = new Order(userId, reservation.Id, payment.Id, payment.Amount, shippingAddress);

        var items = reservation.ReservationItems.Select(ri => (ri.ProductId, ri.Quantity, ri.UnitPrice));
        order.AddItems(items);


        // Increase sold amount for each product
        var productIds = reservation.ReservationItems.Select(ri => ri.ProductId).ToList();
        var products = await _dbContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken: cancellationToken);
        foreach (var product in products)
        {
            product.IncreaseSoldAmount(reservation.ReservationItems.First(ri => ri.ProductId == product.Id).Quantity);
        }

        var result = await _reservationService.FinalizeReservationAsync(reservation, cancellationToken);
        if (result.IsError)
        {
            // Shouldn't happen
            throw new Exception($"Failed to finalize reservation: {result.Errors}");
        }

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order;
    }
}