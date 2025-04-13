namespace EShopApp.Api.Models.Requests;

public class UpdateInventoryRequest
{
    public int ProductId { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }
}