namespace EShopApp.Domain.Entities;

public class InventoryTransaction : Entity<int>
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public InventoryTransactionType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public string Reason { get; set; } = string.Empty;
}
