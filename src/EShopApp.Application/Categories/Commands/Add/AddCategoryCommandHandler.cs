using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Commands.Add;

public record AddCategoryCommand(
    string Name,
    int? ParentId) : IRequest<ErrorOr<Category>>;

public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, ErrorOr<Category>>
{
    private readonly IApplicationDbContext _dbContext;

    public AddCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Category>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? parentCategory = null;

        if (request.ParentId.HasValue)
        {
            parentCategory = await _dbContext.Categories.FindAsync([request.ParentId.Value], cancellationToken);

            if (parentCategory is null)
                return Error.NotFound(description: "Parent category not found.");
        }
        else
        {
            var doesRootCategoryExist = await _dbContext.Categories.AnyAsync(c => c.ParentId == null && c.Name == request.Name, cancellationToken);

            if (doesRootCategoryExist)
                return Error.Conflict(description: "Root category with the same name already exists.");
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var category = parentCategory is null ? new Category(request.Name) : new Category(request.Name, parentCategory);

            await _dbContext.Categories.AddAsync(category, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            category.UpdatePath();
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return category;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Error.Failure(description: e.Message);
        }
    }
}