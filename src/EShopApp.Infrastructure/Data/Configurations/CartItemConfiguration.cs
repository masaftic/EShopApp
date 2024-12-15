using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.OwnsOne(p => p.Price, b =>
        {
            b.Property(m => m.Amount).HasColumnType("decimal(18,2)");
            b.Property(m => m.Currency).HasMaxLength(3);
        });

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}