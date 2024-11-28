using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail =>
            Error.Conflict("User.DuplicateEmail", "Email is already in use.");

        public static Error InvalidCredentials =>
            Error.Validation("User.InvalidCredentials", "Invalid credentials.");
    }
}