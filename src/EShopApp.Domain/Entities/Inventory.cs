namespace EShopApp.Domain.Entities;

public class Inventory : Entity<int>
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Stock { get; set; }
    public int ReservedStock { get; set; }
    public int AvailableStock => Stock - ReservedStock;
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }


    public void Reserve(int quantity)
    {
        if (quantity > AvailableStock)
            throw new InvalidOperationException($"Quantity {quantity} is greater than the available quantity {AvailableStock}");
        
        ReservedStock += quantity;
    }
}
