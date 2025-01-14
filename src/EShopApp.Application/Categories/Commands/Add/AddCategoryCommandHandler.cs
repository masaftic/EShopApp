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
        if (request.Path != string.Empty) 
        {
            var checkUniqueLocalCategory = await _dbContext.Categories.FirstOrDefaultAsync(c =>
                    c.Name == request.Name
                    && c.Path.Length == request.Path.Length + 2 // direct children of request's path
                    && c.Path.StartsWith(request.Path), // same parent path
                    cancellationToken: cancellationToken);

            if (checkUniqueLocalCategory is not null)
                return Error.Conflict("Category.Exists", "Category already exists");

            // Must check if parent category exists to form a valid hierarchy
            var checkExistingParentCategory = await _dbContext.Categories.FirstOrDefaultAsync(c =>
                    c.Path == request.Path, // same parent path
                    cancellationToken: cancellationToken);
            
            if (checkExistingParentCategory is null)
                return Error.Conflict(description: "Category parent not found");
        }

        var category = new Category(request.Name);

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        category.InitPath(request.Path); // Calculate path after generating categoryID
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category;
    }
}