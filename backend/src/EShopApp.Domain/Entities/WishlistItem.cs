using EShopApp.Domain.Errors;

namespace EShopApp.Domain.Entities;


public class WishlistItem : Entity<int>
{
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    private WishlistItem()
    {
    }    

    public WishlistItem(Wishlist wishlist, int productId)
    {
        Wishlist = wishlist;
        ProductId = productId;
    }
}