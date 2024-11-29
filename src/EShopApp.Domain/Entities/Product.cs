using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Product : Entity<Guid>
{
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Product(Guid id) : base(id)
    {
        
    }
    public Product(Guid id, string name, Money price, string description, Guid categoryId)
        : base(id)
    {
        Name = name;
        Quantity = 0;
        Price = price;
        Description = description;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

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