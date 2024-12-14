namespace EShopApp.Api.Models.Requests;

public class GetProductsRequest
{
    public int? CategoryId { get; set; }
    public string? Path { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}