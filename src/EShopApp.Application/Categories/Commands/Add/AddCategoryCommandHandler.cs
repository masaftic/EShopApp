using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Commands.Add;

public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, ErrorOr<Category>>
{
    private readonly IApplicationDbContext _dbContext;

    public AddCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Category>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var parentPath = "";

        if (request.ParentId is not null)
        {
            var parent = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentId,
                cancellationToken: cancellationToken);

            if (parent is null)
                return Error.NotFound(description: "parent category not found");
            
            parentPath = parent.Path;
            
            var checkUniqueLocalCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c =>
                        c.Name == request.Name && // Same name
                        c.Path.Length == parent.Path.Length + 2 && // Direct ancestors of parent
                        c.Path.StartsWith(parent.Path), // Same ancestor path
                    cancellationToken: cancellationToken);

            if (checkUniqueLocalCategory is not null)
                return Error.Conflict("Category.Exists", "Category already exists");
        }
        else
        {
            var checkUniqueRootCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(c =>
                        c.Name == request.Name
                        && c.Path.Length == 2,
                    cancellationToken: cancellationToken);

            if (checkUniqueRootCategory is not null)
                return Error.Conflict("Category.Exists", "Category already exists");
        }

        var category = new Category(request.Name);

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        category.InitPath(parentPath); // Calculate path after generating categoryID
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category;
    }
}