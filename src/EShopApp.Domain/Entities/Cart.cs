using ErrorOr;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Cart : Entity<int>
{
    public int UserId { get; private set; }

    public Cart(int userId)
    {
        UserId = userId;
    }

    public List<CartItem> CartItems { get; private set; } = [];


    public CartItem AddToCart(int productId, int quantity, decimal price)
    {
        var existingCartItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (existingCartItem is not null)
        {
            existingCartItem.AddQuantity(quantity);
            return existingCartItem;
        }

        var cartItem = new CartItem(this.Id, productId, quantity, price);
        CartItems.Add(cartItem);
        return cartItem;
    }
}