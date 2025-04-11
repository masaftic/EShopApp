using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(p => p.SoldAmount).HasDefaultValue(0);

        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");

        builder.Property(p => p.UpdatedAt).HasDefaultValueSql("GETDATE()");
    }
}