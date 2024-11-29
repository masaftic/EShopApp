using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class Errors
{
    public static class Product
    {
        public static Error NotFound => Error.NotFound("Product.NotFound", "Product not found.");
    }
}