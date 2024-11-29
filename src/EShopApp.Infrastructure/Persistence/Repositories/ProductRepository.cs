using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    ApplicationDbContext _dbContext;
    
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId)
    {
        return await _dbContext.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
    }
}