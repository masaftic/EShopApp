using EShopApp.Domain.Entities;
using FluentValidation;

namespace EShopApp.Application.Inventories.Commands.AdjustInventory;

public class AdjustInventoryValidator : AbstractValidator<AdjustInventoryCommand>
{
    public AdjustInventoryValidator()
    {
        RuleFor(x => x.InventoryId)
            .GreaterThan(0).WithMessage("InventoryId must be greater than 0.");

        RuleFor(x => x.TransactionType)
            .Must(type => type is InventoryTransactionType.Inbound or InventoryTransactionType.Adjustment)
            .WithMessage("TransactionType must be \"Inbound\", or \"Adjustment\".");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.")
            .When(x => x.TransactionType != InventoryTransactionType.Release); // Quantity can be 0 for Release.

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(250).WithMessage("Reason cannot exceed 250 characters.");
    }
}