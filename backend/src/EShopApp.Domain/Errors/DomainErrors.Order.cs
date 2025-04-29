using ErrorOr;

namespace EShopApp.Domain.Errors;

public static partial class DomainErrors
{
    public static class Order
    {
        public static Error NotFound => Error.NotFound(
            code: "Order.NotFound",
            description: "The order with the specified ID was not found.");

        public static Error CannotCancel => Error.Validation(
            code: "Order.CannotCancel",
            description: "This order cannot be cancelled due to its current status.");

        public static Error Unauthorized => Error.Unauthorized(
            code: "Order.Unauthorized",
            description: "You are not authorized to perform this action on this order.");
        
        public static Error InvalidStatusTransition => Error.Validation(
            code: "Order.InvalidStatusTransition",
            description: "The status transition is invalid.");
    }
}
