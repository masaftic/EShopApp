namespace EShopApp.Domain.Entities;

public class OrderItem
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; } // Price at the time of order
    
    
    public OrderItem(int productId, string variant, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");
        if (price < 0)
            throw new ArgumentException("Price must not be negative.");

        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}