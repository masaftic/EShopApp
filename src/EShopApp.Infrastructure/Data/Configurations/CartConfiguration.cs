using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId);
        
        // Enforce one-to-one relationship
        builder
            .HasOne<User>() // Cart belongs to one User
            .WithOne(u => u.Cart) // User has one Cart
            .HasForeignKey<Cart>(c => c.UserId) // ForeignKey is UserId
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if User is deleted
    }
}