using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;

namespace EShopApp.Infrastructure.Persistence.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}