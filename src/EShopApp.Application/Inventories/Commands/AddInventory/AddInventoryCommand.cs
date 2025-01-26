using ErrorOr;
using MediatR;

namespace EShopApp.Application.Inventories.Commands.AddInventory;

public record AddInventoryCommand(
    int ProductId,
    int Stock,
    int ReorderLevel,
    int ReorderQuantity) : IRequest<ErrorOr<InventoryDto>>;