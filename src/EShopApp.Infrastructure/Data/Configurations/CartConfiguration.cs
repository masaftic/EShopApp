using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Enforce one-to-one relationship
        builder
            .HasOne<ApplicationUser>() // Cart belongs to one User
            .WithOne() // User has one Cart
            .HasForeignKey<Cart>(c => c.UserId) // ForeignKey is UserId
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete if User is deleted
        
    }
}