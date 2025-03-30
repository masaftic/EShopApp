using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using EShopApp.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Commands;

public record UpdateCategoryCommand(
    int CategoryId,
    string Name,
    int? ParentId) : IRequest<ErrorOr<Updated>>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;

    public UpdateCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Updated>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (category is null)
            return DomainErrors.Category.NotFound(request.CategoryId);
        
        Category? newParent = null;
        if (request.ParentId.HasValue)
        {
            newParent = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);
            if (newParent is null)
                return Error.NotFound(description: "New parent category not found.");
            if (newParent.Id == category.Id || newParent.Path.StartsWith($"{category.Path}/"))
                return Error.Validation(description: "Invalid parent category selection due to circular dependency.");
        }
        
        var oldPath = category.Path;
        
        category.UpdateCategory(request.Name, newParent);
        
        // Update descendant paths
        var descendants = await _dbContext.Categories
            .Where(c => c.Path.StartsWith(oldPath + "/"))
            .ToListAsync(cancellationToken);

        foreach (var descendant in descendants)
        {
            descendant.UpdatePath();
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
    }
}