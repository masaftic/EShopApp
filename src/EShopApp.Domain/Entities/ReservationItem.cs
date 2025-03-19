namespace EShopApp.Domain.Entities;

public class ReservationItem : Entity<int>
{
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    private ReservationItem() // ef core
    {
    }

    public ReservationItem(Reservation reservation, int productId, int quantity, decimal unitPrice)
    {
        Reservation = reservation;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

