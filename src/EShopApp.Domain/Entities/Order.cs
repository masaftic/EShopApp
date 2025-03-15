using System.Dynamic;
using EShopApp.Domain.Enums;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Order : Entity<int>
{
    public int UserId { get; set; }
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; }

    public string OrderNumber { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }

    public int PaymentId { get; set; }
    public Payment Payment { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Shipping details
    // public Address ShippingAddress { get; set; }
    // public string ShippingPostalCode { get; set; }
}
