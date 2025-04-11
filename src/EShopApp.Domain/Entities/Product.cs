using System.Dynamic;
using ErrorOr;
using EShopApp.Domain.Errors;

namespace EShopApp.Domain.Entities;

public class Product : Entity<int>
{
    public string Name { get; private set; } = string.Empty;
    public Inventory Inventory { get; set; } = null!;
    public decimal Price { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public int SoldAmount { get; set; }
    public ICollection<ProductReview> Reviews { get; private set; } = [];
    public double AverageRating { get; set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public ICollection<ProductImage> Images { get; private set; } = [];
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
        AverageRating = 0;
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

    public void IncreaseSoldAmount(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        // SoldAmount += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddReview(ProductReview review)
    {
        Reviews.Add(review);
        AverageRating = Reviews.Average(r => r.Rating);
        UpdatedAt = DateTime.UtcNow;
    }

    public ErrorOr<Deleted> RemoveReview(int reviewId)
    {
        var review = Reviews.FirstOrDefault(r => r.Id == reviewId);
        if (review is null)
            return DomainErrors.Product.ReviewNotFound;

        Reviews.Remove(review);
        AverageRating = Reviews.Count > 0
            ? Reviews.Average(r => r.Rating)
            : 0;

        UpdatedAt = DateTime.UtcNow;
        return Result.Deleted;
    }

    public void AddImage(ProductImage image)
    {
        if (Images.Count == 0)
        {
            image.SetAsMain();
        }
        else if (image.IsMain)
        {
            var currentMain = Images.FirstOrDefault(i => i.IsMain);
            if (currentMain is not null)
            {
                currentMain.SetAsNotMain();
            }
        }

        Images.Add(image);
        UpdatedAt = DateTime.UtcNow;
    }

    public ErrorOr<Deleted> RemoveImage(int imageId)
    {
        var image = Images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return DomainErrors.Product.ImageNotFound;

        Images.Remove(image);

        if (image.IsMain && Images.Count > 0)
        {
            // Set the first image as main if it exists
            Images.First().SetAsMain();
        }

        UpdatedAt = DateTime.UtcNow;
        return Result.Deleted;
    }

    public void SetMainImage(int imageId)
    {
        var currentMain = Images.FirstOrDefault(i => i.IsMain);
        if (currentMain is not null)
        {
            currentMain.SetAsNotMain();
        }

        var newMain = Images.FirstOrDefault(i => i.Id == imageId);
        if (newMain is not null)
        {
            newMain.SetAsMain();
            UpdatedAt = DateTime.UtcNow;
        }
    }

}