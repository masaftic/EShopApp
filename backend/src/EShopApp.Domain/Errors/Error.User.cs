using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class User
    {
        public static Error DuplicateEmail =>
            Error.Conflict("User.Email", "Email is already in use");

        public static Error InvalidCredentials =>
            Error.Unauthorized("User.Password", "Invalid credentials");

        public static Error NotFound =>
            Error.NotFound("User.NotFound", "User not found");
    }
}
