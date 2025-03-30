namespace EShopApp.Domain.Entities;

public class RefreshToken : Entity<Guid>
{
    public string Token { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ExpiresOnUtc { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;

    public RefreshToken(string token, int userId, DateTime expiresOnUtc)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public void Update(string token, DateTime expiresOnUtc)
    {
        Token = token;
        ExpiresOnUtc = expiresOnUtc;
    }
}