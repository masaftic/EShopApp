namespace EShopApp.Domain.Entities;

public class Inventory : Entity<int>
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Stock { get; set; }
    public int ReservedStock { get; set; }
    public int AvailableStock => Stock - ReservedStock;
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }

    private Inventory() // ef core
    {
    }

    public Inventory(int productId, int stock, int reorderQuantity, int reorderLevel)
    {
        ProductId = productId;
        Stock = stock;
        ReservedStock = 0;
        ReorderLevel = reorderLevel;
        ReorderQuantity = reorderQuantity;
    }

    public void IncreaseStock(int quantity)
    {
        Stock += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity > Stock)
            throw new InvalidOperationException(
                $"Quantity {quantity} is greater than the current stock {Stock}");

        ReservedStock -= quantity;
        Stock -= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity > AvailableStock)
            throw new InvalidOperationException(
                $"Quantity {quantity} is greater than the available quantity {AvailableStock}");

        ReservedStock += quantity;
    }

    public void Release(int quantity)
    {
        if (quantity > ReservedStock)
            throw new InvalidOperationException(
                $"Quantity {quantity} is greater than the reserved quantity {ReservedStock}");

        ReservedStock -= quantity;
    }
}