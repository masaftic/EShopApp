namespace EShopApp.Domain.Entities;

public abstract class LineItem : Entity<int>
{
    public int ProductId { get; private set; }
    public Product? Product { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    protected LineItem()
    {
    }

    protected LineItem(int productId, decimal unitPrice, int quantity)
    {
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity must be greater than or equal to 0.");

        Quantity = quantity;
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price must be greater than or equal to 0.");

        UnitPrice = price;
    }

    public void AddQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.");

        Quantity += quantity;
    }
}
