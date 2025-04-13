namespace EShopApp.Application.Products.DTOs;

public class ProductReviewDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string Comment { get; set; } = "";
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}