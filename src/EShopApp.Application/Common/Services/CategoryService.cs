using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Common.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _dbContext;

    public CategoryService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<List<Category>>> GetByPathIdsAsync(List<int> pathIds)
    {
        foreach (var id in pathIds)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return Errors.Category.NotFound(id);
        }

        return await RetrieveCategoriesFromHierarchyPath(pathIds);
    }

    public async Task<ErrorOr<List<Category>>> GetByPathNamesAsync(List<string> names)
    {
        var pathIds = new List<int>();

        for (var i = 0; i < names.Count; i++)
        {
            // /laptops/lenovo
            // /1/2
            // because names can match under different subtrees
            // name position must be the same as path length
            // name position is multiplied by 2 because of '/' characters

            var name = names[i].ToLower();
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(c => 
                    c.Path.Length == (i + 1) * 2 &&
                    c.Name.ToLower() == name);

            if (category == null)
                return Errors.Category.PathNotFound;

            pathIds.Add(category.Id);
        }

        return await RetrieveCategoriesFromHierarchyPath(pathIds);
    }

    private async Task<ErrorOr<List<Category>>> RetrieveCategoriesFromHierarchyPath(List<int> pathIds)
    {
        var path = $"/{string.Join("/", pathIds)}";
        var categories = await _dbContext.Categories.Where(c => c.Path == path || (path != "/" && c.Path.StartsWith(path + "/"))).ToListAsync();

        if (categories.Count == 0)
            return Errors.Category.PathNotFound;

        return categories;
    }
}