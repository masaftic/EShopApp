using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
        
        builder
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        
        // builder.OwnsOne(o => o.ShippingAddress, address =>
        // {
        //     address.Property(a => a.Street).IsRequired().HasMaxLength(200);
        //     address.Property(a => a.City).IsRequired().HasMaxLength(100);
        //     address.Property(a => a.Country).IsRequired().HasMaxLength(100);
        // });
    }
}