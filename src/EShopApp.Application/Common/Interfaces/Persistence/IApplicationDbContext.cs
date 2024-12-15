using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}