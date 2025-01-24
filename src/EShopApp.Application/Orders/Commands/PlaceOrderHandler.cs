using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Orders.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Enums;
using EShopApp.Domain.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Commands;

public record PlaceOrderCommand(Address ShippingAddress, string ShippingPostalCode) : IRequest<ErrorOr<OrderDto>>;

public class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, ErrorOr<OrderDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public PlaceOrderHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var userCart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (userCart is null)
            return Error.NotFound(description: "Cart not found");

        if (userCart.SessionExpiryDate is null || userCart.SessionExpiryDate < DateTime.UtcNow)
            return Error.Conflict(description: "Session expired, Please checkout again");

        var activeReservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Status == ReservationStatus.Active, cancellationToken);
        if (activeReservation == null)
            return Error.Conflict(description: "Reservation expired or invalid.");


        var order = new Order
        {
            UserId = userCart.UserId,
            OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
            TotalAmount = userCart.TotalPrice,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ShippingAddress = request.ShippingAddress,
            ShippingPostalCode = request.ShippingPostalCode,
            OrderItems = userCart.CartItems.Select(ci => new OrderItem()
            {
                ProductId = ci.ProductId, // Ensure this is valid
                Quantity = ci.Quantity, // Ensure this is valid
                UnitPrice = ci.UnitPrice // Ensure this is valid
            }).ToArray()
        };
        
        
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        _dbContext.Carts.Remove(userCart);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return order.Adapt<OrderDto>();
    }
}