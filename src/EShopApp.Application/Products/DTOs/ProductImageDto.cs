namespace EShopApp.Application.Products.DTOs;

public record ProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}


