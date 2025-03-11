using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Domain.Entities.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Payment> builder)
    {
        builder
            .HasOne<User>()
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId);
    }
}
