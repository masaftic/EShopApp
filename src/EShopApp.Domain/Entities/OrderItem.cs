using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class OrderItem : Entity<int>
{
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Price at the time of order
    public decimal TotalPrice => Quantity * UnitPrice;
}