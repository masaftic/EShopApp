using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Product : Entity<int>
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }
    public string Description { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Product()
    {
    }
    
    public Product(string name, Money price, string description, int categoryId)
    {
        Name = name;
        Quantity = 0;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateProduct(string name, int quantity, Money price, string description, int categoryId)
    {
        Name = name;
        Quantity = quantity;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void DeleteProduct()
    {
        DeletedAt = DateTime.UtcNow;
    }

    // TODO: Domain Errors
    public void AddStock(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Stock amount must be greater than 0.");
        Quantity += amount;
    }

    public void ReduceStock(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0.");
        if (amount > Quantity)
            throw new InvalidOperationException("Insufficient stock.");
        Quantity -= amount;
    }

    public void MarkAsDeleted()
    {
        DeletedAt = DateTime.UtcNow;
    }

    public bool IsAvailable() => DeletedAt == null && Quantity > 0;
}