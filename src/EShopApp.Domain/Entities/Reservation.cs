namespace EShopApp.Domain.Entities;

public class Reservation : Entity<int>
{
    public int UserId { get; set; }
    public string PaymentIntentId { get; set; } = null!;
    public ReservationStatus Status { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ReservationItem> ReservationItems { get; set; }
}

