namespace EShopApp.Application.Products.DTOs
{
    public class ProductPreviewDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int SoldAmount { get; set; }
        public double AverageRating { get; set; }
        public string CategoryName { get; set; } = "";
        public string? ThumbnailUrl { get; set; }
    }
}
