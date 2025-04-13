using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class Inventory
    {
        public static Error NotFound(int id) =>
            Error.NotFound("Inventory.NotFound", $"Inventory with id: '{id}' was not found.");
    }
}