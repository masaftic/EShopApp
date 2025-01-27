using ErrorOr;
using MediatR;

namespace EShopApp.Application.Inventories.Commands.AddInventory;

public record UpdateInventoryCommand(
    int InventoryId,
    int ProductId,
    int ReorderLevel,
    int ReorderQuantity) : IRequest<ErrorOr<Success>>;