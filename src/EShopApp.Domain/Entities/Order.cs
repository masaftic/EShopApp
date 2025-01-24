using System.Dynamic;
using EShopApp.Domain.Enums;
using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class Order : Entity<int>
{
    public int UserId { get; set; }
    public string OrderNumber { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? CanceledAt { get; set; }


    // Shipping details
    public Address ShippingAddress { get; set; }
    public string ShippingPostalCode { get; set; }


    // Future payment details
    public string PaymentMethod { get; set; } = "";
    public string TransactionId { get; set; } = "";
}
