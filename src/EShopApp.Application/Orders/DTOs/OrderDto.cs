using EShopApp.Application.Common.DTOs;

namespace EShopApp.Application.Orders.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public AddressDto ShippingAddress { get; set; } = null!;
    public string ShippingPostalCode { get; set; } = "";
    public OrderItemDto[] OrderItems { get; set; } = [];
}