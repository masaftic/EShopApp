namespace EShopApp.Domain.Entities;

// TODO: validating domain entities creation
public class Category : Entity<Guid>
{
    public string Name { get; private set; }
    // TODO: Hierarchical categories
    // public string Path { get; private set; } // e.g., "1/2/4" for "Laptops"
    public DateTime CreatedAt { get; private set; }

    public Category(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }
    // public List<int> GetCategoriesIdsFromPath()
    // {
    //     return Path.Split('/').Select(int.Parse).ToList();
    // }
    //
    // // Method to check if a category is a child of another category
    // public bool IsChildOf(Category parentCategory)
    // {
    //     return Path.StartsWith(parentCategory.Path);
    // }
}