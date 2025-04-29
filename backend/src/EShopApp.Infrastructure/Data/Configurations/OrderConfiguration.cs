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

        builder.OwnsOne(u => u.ShippingAddress, address =>
        {
            address.Property(a => a.StreetLine1).HasColumnName("AddressLine1").HasMaxLength(100);
            address.Property(a => a.StreetLine2).HasColumnName("AddressLine2").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(50);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(50);
            address.Property(a => a.PostalCode).HasColumnName("ZipCode").HasMaxLength(20);
        });
    }
}