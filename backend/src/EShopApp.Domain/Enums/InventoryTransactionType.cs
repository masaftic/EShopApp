namespace EShopApp.Domain.Entities;

public enum InventoryTransactionType
{
    Inbound,  // Stock added (e.g., received from supplier).
    Outbound, // Stock removed (e.g., sold or consumed).
    Reserve,  // Stock reserved for orders.
    Release,  // Stock reservation canceled.
    Adjustment // Manual inventory adjustment (e.g., for discrepancies).
}
