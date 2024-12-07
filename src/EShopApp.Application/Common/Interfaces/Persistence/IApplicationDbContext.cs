using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
