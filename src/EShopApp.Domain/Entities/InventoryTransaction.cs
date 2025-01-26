namespace EShopApp.Domain.Entities;

public class InventoryTransaction : Entity<int>
{
    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;
    public int Quantity { get; set; }
    public InventoryTransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = string.Empty;

    public InventoryTransaction(int inventoryId, int quantity, InventoryTransactionType type, DateTime timestamp, string reason)
    {
        InventoryId = inventoryId;
        Quantity = quantity;
        Type = type;
        Timestamp = timestamp;
        Reason = reason;
    }
}
