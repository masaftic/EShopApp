using ErrorOr;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Cart : Entity<int>
{
    public int UserId { get; private set; }
    public User? User { get; private set; }
    public DateTime? SessionExpiryDate { get; private set; }
    public static TimeSpan DefaultSessionExpiryDuration = TimeSpan.FromMinutes(15);
    public ICollection<CartItem> CartItems { get; private set; } = [];
    public decimal TotalPrice => CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);

    private Cart()
    {
    }

    public Cart(int userId)
    {
        UserId = userId;
    }

    public void SetExpiryDate(DateTime expiryDate)
    {
        SessionExpiryDate = expiryDate;
    }

    public CartItem AddToCart(int productId, int quantity, decimal price)
    {
        var existingCartItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (existingCartItem is not null)
        {
            existingCartItem.AddQuantity(quantity);
            return existingCartItem;
        }

        var cartItem = new CartItem(this, productId, quantity, price);
        CartItems.Add(cartItem);
        return cartItem;
    }

    public void ClearCart()
    {
        CartItems.Clear();
        SessionExpiryDate = null;
    }
}