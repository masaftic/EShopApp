using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class Wishlist
    {
        public static Error ItemAlreadyExists(int productId) => Error.Conflict("Wishlist.ItemAlreadyExists", $"Item: {productId} already exists in the wishlist");
    }
}