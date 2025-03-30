namespace EShopApp.Domain.Entities;

using ErrorOr;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;

public class Wishlist : Entity<int>
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];

    private Wishlist()
    {
    }

    public Wishlist(int userId)
    {
        UserId = userId;
    }

    public ErrorOr<Success> AddItem(int productId)
    {
        if (WishlistItems.Any(item => item.ProductId == productId))
            return DomainErrors.Wishlist.ItemAlreadyExists(productId);

        WishlistItems.Add(new WishlistItem(this, productId));
        return Result.Success;
    }

    public void RemoveItem(int productId)
    {
        var item = WishlistItems.FirstOrDefault(item => item.ProductId == productId);
        if (item != null)
        {
            WishlistItems.Remove(item);
        }
    }
}

