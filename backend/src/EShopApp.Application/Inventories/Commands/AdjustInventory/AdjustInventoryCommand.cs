using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Inventories.Commands.AdjustInventory;

public record AdjustInventoryCommand(
    int InventoryId,
    InventoryTransactionType TransactionType,
    int Quantity,
    string Reason) : IRequest<ErrorOr<Success>>;
