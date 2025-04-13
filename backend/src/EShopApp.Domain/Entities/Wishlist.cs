namespace EShopApp.Domain.Entities;

using System;
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

    public ErrorOr<Deleted> RemoveItem(int productId)
    {
        var item = WishlistItems.FirstOrDefault(item => item.ProductId == productId);
        if (item is null)
        {
            return DomainErrors.Wishlist.ItemNotFound(productId);
        }

        WishlistItems.Remove(item);
        return Result.Deleted;
    }

    public void Clear()
    {
        WishlistItems.Clear();
    }
}

