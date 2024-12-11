using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class Errors
{
    public static class Category
    {
        public static Error NotFound(int id) =>
            Error.NotFound("Category.NotFound", $"Category with id: '{id}' was not found.");

        public static Error PathNotFound =>
            Error.NotFound("Category.Path.NotFound", "No categories found for the given path.");
    }
}