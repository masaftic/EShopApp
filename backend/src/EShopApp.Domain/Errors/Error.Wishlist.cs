using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class Wishlist
    {
        public static Error ItemAlreadyExists(int productId) => Error.Conflict("Wishlist.ItemAlreadyExists", $"Item: {productId} already exists in the wishlist");

        public static Error ItemNotFound(int productId) => Error.NotFound("Wishlist.ItemNotFound", $"Item: {productId} not found in the wishlist");
    }
}