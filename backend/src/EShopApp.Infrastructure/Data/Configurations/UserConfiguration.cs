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
            address.Property(a => a.StreetLine1).HasColumnName("AddressLine1").HasMaxLength(100);
            address.Property(a => a.StreetLine2).HasColumnName("AddressLine2").HasMaxLength(100);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(50);
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(50);
            address.Property(a => a.PostalCode).HasColumnName("ZipCode").HasMaxLength(20);
        });

        builder
            .HasOne<ApplicationUser>()
            .WithOne(u => u.User)
            .HasForeignKey<ApplicationUser>(u => u.UserId);
    }
}