using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany<ReservationItem>(r => r.ReservationItems)
            .WithOne()
            .HasForeignKey(ri => ri.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(o => o.PaymentIntentId).IsRequired();
    }
}

public class ReservationItemConfiguration : IEntityTypeConfiguration<ReservationItem>
{
    public void Configure(EntityTypeBuilder<ReservationItem> builder)
    {
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}