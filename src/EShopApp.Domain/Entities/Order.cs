using EShopApp.Domain.Enums;

namespace EShopApp.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public OrderState State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    
    
    public Order(int userId, List<OrderItem> items)
    {
        UserId = userId;
        Items = items ?? throw new ArgumentNullException(nameof(items));
        State = OrderState.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void ShipOrder()
    {
        if (State != OrderState.Pending)
            throw new InvalidOperationException("Only pending orders can be shipped.");
        State = OrderState.Shipped;
        ShippedAt = DateTime.UtcNow;
    }

    public void CancelOrder()
    {
        if (State != OrderState.Pending)
            throw new InvalidOperationException("Only pending orders can be canceled.");
        State = OrderState.Canceled;
        CanceledAt = DateTime.UtcNow;
    }

    public decimal TotalPrice()
    {
        return Items.Sum(item => item.Price * item.Quantity);
    }
}