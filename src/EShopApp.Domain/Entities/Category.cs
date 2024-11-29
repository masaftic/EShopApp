namespace EShopApp.Domain.Entities;

public class Category : Entity<Guid>
{
    public string Name { get; private set; }

    // TODO: Hierarchical categories
    // public string Path { get; private set; } // e.g., "/1/2/4" for "Laptops"
    public DateTime CreatedAt { get; private set; }

    public Category(Guid id, string name) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        // Path = path ?? throw new ArgumentNullException(nameof(path));
        CreatedAt = DateTime.UtcNow;
    }

    // Method to check if a category is a child of another category
    // public bool IsChildOf(Category parentCategory)
    // {
    //     return Path.StartsWith(parentCategory.Path);
    // }
}