using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

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
        var checkUniqueLocalCategory = _dbContext.Categories.FirstOrDefault(
            c =>
                c.Name == request.Name
                && c.Path.Length == request.Path.Length + 2 // direct children of request's path
                && c.Path.StartsWith(request.Path)); // same parent path
        
        if (checkUniqueLocalCategory is not null)
            return Error.Conflict("Category.Exists", "Category already exists");

        var category = new Category(request.Name);
        
        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        category.InitPath(request.Path); // Calculate path after generating categoryID
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return category;
    }
}