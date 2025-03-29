using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class User
    {
        public static Error DuplicateEmail =>
            Error.Conflict("User.DuplicateEmail", "Email is already in use");

        public static Error InvalidCredentials =>
            Error.Conflict("User.InvalidCredentials", "Invalid credentials");

        public static Error NotFound =>
            Error.NotFound("User.NotFound", "User not found");
    }
}
