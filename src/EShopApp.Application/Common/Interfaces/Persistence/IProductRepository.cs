using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IProductRepository : IRepository<Product>
{ 
    Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId);    
}
