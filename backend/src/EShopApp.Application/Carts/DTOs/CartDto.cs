using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Carts.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemDto> CartItems { get; set; } = [];
    public decimal TotalPrice => CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
}

