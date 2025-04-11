namespace EShopApp.Infrastructure.Data.Configurations;

using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.Property(r => r.Comment)
            .IsRequired()
            .HasMaxLength(500);
    }
}
