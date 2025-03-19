namespace EShopApp.Domain.Entities;

// TODO: validating domain entities creation
public class Category : Entity<int>
{
    public string Name { get; private set; }
    public int? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public string Path { get; private set; } = string.Empty; // e.g., "/1/2/4" for "Laptops"
    public DateTime CreatedAt { get; private set; }

    public Category(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow; 
    }

    public Category(string name, Category parent)
    {
        Name = name;
        Parent = parent;
        ParentId = parent.Id;
        CreatedAt = DateTime.UtcNow;
    }

    // Must be called after initializing ID.
    public void UpdatePath()
    {
        if (ParentId.HasValue)
        {
            Path = $"{Parent!.Path}/{Id}";
        }
        else
        {
            Path = $"/{Id}";
        }
    }

    public void SetIdAndPath(int id, string path)
    {
        this.Id = id;
        this.Path = path;
    }
}