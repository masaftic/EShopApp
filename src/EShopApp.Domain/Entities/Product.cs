using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Product : Entity<int>
{
    public string Name { get; private set; }
    public Inventory Inventory { get; set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    
    public Product()
    {
    }

    public Product(int id, string name, decimal price, string description, int categoryId)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Product(string name, decimal price, string description, int categoryId)
    {
        Name = name;
        Price = price;
        Description = description;
        CategoryId = categoryId;
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