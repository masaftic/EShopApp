using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.OwnsOne(p => p.Price, b =>
        {
            b.Property(m => m.Amount).HasColumnType("decimal(18,2)");
            b.Property(m => m.Currency).HasMaxLength(3);
        });
    }
}