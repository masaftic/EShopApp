namespace EShopApp.Application.Products.DTOs
{
    public class ProductPreviewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = "";
        public string? ThumbnailUrl { get; set; }
    }
}
