namespace EShopApp.Domain.Entities;

public class ProductReview : Entity<int>
{
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Comment { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductReview() // ef core
    {
    }

    public ProductReview(int productId, int userId, string comment, int rating)
    {
        if (string.IsNullOrEmpty(comment)) throw new ArgumentException("Comment is required.");
        if (rating < 1 || rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");

        ProductId = productId;
        UserId = userId;
        Comment = comment;
        Rating = rating;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateReview(string comment, int rating)
    {
        Comment = comment;
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }
}