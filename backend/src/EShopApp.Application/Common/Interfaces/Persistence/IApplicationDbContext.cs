using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductReview> ProductReviews { get; }
    DbSet<Category> Categories { get; }
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<ReservationItem> ReservationItems { get; }
    DbSet<Inventory> Inventories { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<Payment> Payments { get; }
    DbSet<User> DomainUsers { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Wishlist> Wishlists { get; }
    DbSet<WishlistItem> WishlistItems { get; }
    
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}