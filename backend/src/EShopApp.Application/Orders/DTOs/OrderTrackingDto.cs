using EShopApp.Domain.Enums;

namespace EShopApp.Application.Orders.DTOs;

public record OrderTrackingDto(
    int OrderId,
    string OrderNumber,
    OrderStatus Status
);
