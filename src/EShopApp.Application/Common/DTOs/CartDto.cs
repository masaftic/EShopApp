using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.DTOs;

public class CartDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public List<CartItemDto> CartItems { get; set; }
}


public static class CartMappingExtensions
{
    public static CartDto ToDto(this Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CartItems = cart.CartItems.Select(ci => ci.ToDto()).ToList()
        };
    }

    public static CartItemDto ToDto(this CartItem cartItem)
    {
        return new CartItemDto
        {
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            Price = cartItem.Price.Amount // Assuming Money has Amount
        };
    }
}
