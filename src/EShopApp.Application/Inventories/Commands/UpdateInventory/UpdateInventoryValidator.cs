using FluentValidation;

namespace EShopApp.Application.Inventories.Commands.AddInventory;

public class UpdateInventoryValidator : AbstractValidator<AddInventoryCommand>
{
    public UpdateInventoryValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be 0 or greater.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("ReorderLevel must be 0 or greater.");

        RuleFor(x => x.ReorderQuantity)
            .GreaterThan(0).WithMessage("ReorderQuantity must be greater than 0.");
    }
}