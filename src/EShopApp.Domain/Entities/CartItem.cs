using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class CartItem : Entity<int>
{
    public int CartId { get; private set; }
    public Cart Cart { get; private set; } = null!;

    public int ProductId { get; private set; }
    public Product? Product { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; } // Price at time of checkout

    private CartItem() // ef core
    {
    }

    // TODO: How to handle variants?
    public CartItem(Cart cart, int productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");
        if (unitPrice <= 0)
            throw new ArgumentException("Price must be greater than 0.");
        
        Cart = cart;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public void UpdateItem(int quantity, decimal price)
    {
        Quantity = quantity;
        UnitPrice = price;
    }

    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");
        Quantity += quantity;
    }

    public void UpdatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than 0.");
        UnitPrice = price;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        Quantity = quantity;
    }
}