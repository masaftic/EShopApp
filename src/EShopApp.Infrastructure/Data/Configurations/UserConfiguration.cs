using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.Address, address =>
        {
            address.Property(a => a.Street).IsRequired().HasMaxLength(200);
            address.Property(a => a.City).IsRequired().HasMaxLength(100);
            address.Property(a => a.Country).IsRequired().HasMaxLength(100);
        });

        builder
            .HasOne<ApplicationUser>()
            .WithOne(u => u.User)
            .HasForeignKey<ApplicationUser>(u => u.UserId);
    }
}