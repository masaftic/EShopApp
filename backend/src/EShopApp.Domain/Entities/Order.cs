using System.Dynamic;
using EShopApp.Domain.Enums;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Order : Entity<int>
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int ReservationId { get; set; }
    public Reservation? Reservation { get; set; }

    public string OrderNumber { get; set; } = string.Empty;
    public ICollection<OrderItem> OrderItems { get; set; } = [];

    public int PaymentId { get; set; }
    public Payment? Payment { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    private Order() // ef core
    {
    }

    public Order(int userId, int reservationId, int paymentId, decimal totalAmount)
    {
        UserId = userId;
        ReservationId = reservationId;
        PaymentId = paymentId;
        OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}";
        TotalAmount = totalAmount;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItems(IEnumerable<(int productId, int quantity, decimal unitPrice)> items)
    {
        if (items == null || !items.Any()) throw new ArgumentException("Order must have items.");

        foreach (var (productId, quantity, unitPrice) in items)
        {
            OrderItems.Add(new OrderItem(this, productId, quantity, unitPrice));
        }
    }

    // Shipping details
    // public Address ShippingAddress { get; set; }
    // public string ShippingPostalCode { get; set; }
}
