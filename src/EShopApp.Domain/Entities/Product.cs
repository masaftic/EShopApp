using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Product : Entity<int>
{
    public string Name { get; private set; } = string.Empty;
    public Inventory Inventory { get; set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    
    private Product() // ef core
    {
    }

    public Product(string name, decimal price, string description, Category category)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Product name is required.");
        if (price <= 0) throw new ArgumentException("Product price must be greater than zero.");

        Name = name;
        Price = price;
        Description = description;
        Category = category ?? throw new ArgumentException("Product category is required.");
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProduct(string name, decimal price, string description, int categoryId)
    {
        Name = name;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}