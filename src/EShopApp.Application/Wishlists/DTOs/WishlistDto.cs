namespace EShopApp.Application.Wishlists.DTOs;

public class WishlistDto
{
    public int Id { get; set; }
    public List<WishlistItemDto> Items { get; set; } = [];
}

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal Price { get; set; }
}