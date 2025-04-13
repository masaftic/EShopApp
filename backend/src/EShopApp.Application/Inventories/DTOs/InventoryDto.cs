namespace EShopApp.Application.Inventories.Commands.AddInventory;

public class InventoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Stock { get; set; }
    public int ReservedStock { get; set; }
    public int AvailableStock => Stock - ReservedStock;
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
}