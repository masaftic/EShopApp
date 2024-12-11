using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ErrorOr<List<Category>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCategoriesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<List<Category>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Segments == null)
            return await _dbContext.Categories.ToListAsync(cancellationToken: cancellationToken);

        var countOfParsableSegments = request.Segments.Count(s => int.TryParse(s, out _));
        
        if (countOfParsableSegments == request.Segments.Length)
        {
            var ids = request.Segments.Select(int.Parse).ToList();
            return await ProcessByIds(ids);
        }
        else if (countOfParsableSegments == 0)
        {
            var names = request.Segments.Select(s => s.ToLower()).ToList();
            return await ProcessByNames(names);
        }
        else
        {
            return Error.Validation("Category.Path.Types.Inconsistent", "Inconsistent Category Path");
        }
    }


    private async Task<ErrorOr<List<Category>>> ProcessByIds(List<int> ids)
    {
        foreach (var id in ids)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category == null)
                return Errors.Category.NotFound(id);
        }
        
        var path = $"/{string.Join("/", ids)}";
        var categories = await _dbContext.Categories.Where(c => c.Path.StartsWith(path)).ToListAsync();

        if (categories.Count == 0)
            return Errors.Category.PathNotFound;
        
        return categories;
    }

    private async Task<ErrorOr<List<Category>>> ProcessByNames(List<string> names)
    {
        throw new NotImplementedException();
    }
}