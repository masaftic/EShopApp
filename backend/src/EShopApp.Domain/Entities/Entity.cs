namespace EShopApp.Domain.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }

#pragma warning disable CS8618
    protected Entity()
#pragma warning restore CS8618
    {
    }
}
