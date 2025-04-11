namespace EShopApp.Application.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int SoldAmount { get; set; }
    public double AverageRating { get; set; }
    public IReadOnlyList<ProductReviewDto> Reviews { get; set; } = [];
    public IReadOnlyList<ProductImageDto> Images { get; set; } = [];
}
