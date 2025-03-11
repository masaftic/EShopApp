
namespace EShopApp.Domain.Entities;

public class Payment : Entity<int>
{
    public int UserId { get; set; }
    // public int OrderId {get; set; }
    public required string PaymentIntentId { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
