using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopApp.Infrastructure.Data.Configurations;

public class InventoryTransactionsConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasOne<Inventory>(it => it.Inventory)
            .WithMany()
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}