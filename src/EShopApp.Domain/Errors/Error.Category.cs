using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class Errors
{
    public static class Category
    {
        public static Error NotFound => Error.NotFound("Category.NotFound", "Category not found.");
    }
}