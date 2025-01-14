using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Helpers;

public class CategoryPathProcessor
{
    private readonly ICategoryService _categoryService;

    public CategoryPathProcessor(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Returns all categories in the segments path after parsing the segments
    /// </summary>
    /// <param name="segments"></param>
    /// <returns></returns>
    public async Task<ErrorOr<List<Category>>> ProcessSegmentsAsync(string[] segments)
    {
        var numericSegmentsCount = segments.Count(s => int.TryParse(s, out _));

        if (numericSegmentsCount == segments.Length)
        {
            var ids = segments.Select(int.Parse).ToList();
            return await _categoryService.GetByPathIdsAsync(ids);
        }

        if (numericSegmentsCount == 0)
        {
            var names = segments.Select(s => s.ToLower()).ToList();
            return await _categoryService.GetByPathNamesAsync(names);
        }

        return Error.Validation("Category.Path.Types.Inconsistent", "Inconsistent Category Path");
    }
}