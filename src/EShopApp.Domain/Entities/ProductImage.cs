namespace EShopApp.Domain.Entities;

public class ProductImage : Entity<int>
{
    public string ImageKey { get; private set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public bool IsMain { get; private set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductImage() { } // EF Core

    public ProductImage(string imageKey, string originalFileName, bool isMain = false)
    {
        if (string.IsNullOrEmpty(imageKey))
            throw new ArgumentException("Image URL is required");
        
        if (string.IsNullOrEmpty(originalFileName))
            throw new ArgumentException("Original file name is required");


        ImageKey = imageKey;
        OriginalFileName = originalFileName;
        IsMain = isMain;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImageKey(string newImageKey)
    {
        if (string.IsNullOrEmpty(newImageKey))
            throw new ArgumentException("Image URL is required");

        ImageKey = newImageKey;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsMain()
    {
        IsMain = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsNotMain()
    {
        IsMain = false;
        UpdatedAt = DateTime.UtcNow;
    }
}