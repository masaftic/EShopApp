using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class Product
    {
        public static Error NotFound => Error.NotFound("Product.NotFound", "Product not found.");

        public static Error TooManyImages => Error.Validation(
            code: "Product.TooManyImages",
            description: "Product has too many images. Maximum is 5.");
        
        public static Error ImageNotFound => Error.NotFound("Product.Image.NotFound", "product image not found");

        public static Error ReviewNotFound => Error.NotFound("Product.Review.NotFound", "product review not found");

        public static Error ReviewAlreadyExists => Error.Conflict("Product.Review.AlreadyExists", "product review already exists");
    }
}