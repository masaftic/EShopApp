namespace EShopApp.Domain.Entities;

public class Reservation : Entity<int>
{
    public int UserId { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public ReservationStatus Status { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ReservationItem> ReservationItems { get; set; } = [];

    private Reservation() // ef core
    {
    }

    public Reservation(int userId, string paymentIntentId)
    {
        UserId = userId;
        PaymentIntentId = paymentIntentId;
        Status = ReservationStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        ExpirationDate = DateTime.UtcNow.AddMinutes(15);
    }

    public void AddReservationItem(int productId, int quantity, decimal unitPrice)
    {
        ReservationItems.Add(new ReservationItem(this, productId, quantity, unitPrice));
    }

    public void AddItems(IEnumerable<(int productId, int quantity, decimal unitPrice)> items)
    {
        if (items == null || !items.Any()) throw new ArgumentException("Order must have items.");

        foreach (var (productId, quantity, unitPrice) in items)
        {
            ReservationItems.Add(new ReservationItem(this, productId, quantity, unitPrice));
        }
    }
}

