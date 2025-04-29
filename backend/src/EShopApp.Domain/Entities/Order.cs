using System.Dynamic;
using EShopApp.Domain.Enums;
using EShopApp.Domain.Errors;
using EShopApp.Domain.ValueObjects;
using ErrorOr;

namespace EShopApp.Domain.Entities;

public class Order : Entity<int>
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public Address ShippingAddress { get; set; } = Address.Default;

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

    public Order(int userId, int reservationId, int paymentId, decimal totalAmount, Address shippingAddress)
    {
        UserId = userId;
        ReservationId = reservationId;
        PaymentId = paymentId;
        TotalAmount = totalAmount;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}";
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

    public ErrorOr<Success> Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
        {
            return DomainErrors.Order.CannotCancel;
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success;
    }

}
