using EShopApp.Domain.Entities;

namespace EShopApp.Api.Models.Requests;

public record AdjustInventoryRequest(
    InventoryTransactionType AdjustmentType,
    int Quantity,
    string Reason);