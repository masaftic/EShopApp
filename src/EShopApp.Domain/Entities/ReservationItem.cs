namespace EShopApp.Domain.Entities;

public class ReservationItem : Entity<int>
{
    public int ReservationId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

