namespace EShopApp.Domain.Entities;

public class ReservationItem : LineItem
{
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;

    private ReservationItem() // ef core
    {
    }

    public ReservationItem(Reservation reservation, int productId, int quantity, decimal unitPrice) : base(productId, unitPrice, quantity)
    {
        Reservation = reservation;
    }
}

