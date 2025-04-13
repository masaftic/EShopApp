using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class CartItem : LineItem
{
    public int CartId { get; private set; }
    public Cart Cart { get; private set; } = null!;

    private CartItem()
    {
    }

    public CartItem(Cart cart, int productId, int quantity, decimal unitPrice) : base(productId, unitPrice, quantity)
    {
        Cart = cart;
    }
}