using EShopApp.Domain.ValueObjects;

namespace EShopApp.Domain.Entities;

public class OrderItem : LineItem
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    private OrderItem() // ef core
    {
    }

    public OrderItem(Order order, int productId, int quantity, decimal unitPrice) : base(productId, unitPrice, quantity)
    {
        Order = order;
    }
}