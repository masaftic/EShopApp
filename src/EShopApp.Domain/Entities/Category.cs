namespace EShopApp.Domain.Entities;

// TODO: validating domain entities creation
public class Category : Entity<int>
{
    public string Name { get; private set; }
    public string Path { get; private set; } = string.Empty; // e.g., "/1/2/4" for "Laptops"
    public DateTime CreatedAt { get; private set; }

    public Category(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    // Must be called after initializing ID.
    public void InitPath(string parentPath)
    {
        if (Path == string.Empty && Id != 0) 
            Path = $"{parentPath}/{Id}";
    }
    
    public void SetIdAndPath(int id, string path)
    {
        this.Id = id;
        this.Path = path;
    }
}