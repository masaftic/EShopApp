using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class CartItem : Entity<int>
{
    public int CartId { get; private set; }
    public Cart Cart { get; private set; }

    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; } // Price at the time of order


    private CartItem()
    {
    }

    // TODO: How to handle variants?
    public CartItem(int cartId, int productId, int quantity, Money price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }


    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}