using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Commands.Add;

public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, ErrorOr<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public AddCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ErrorOr<Category>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(Guid.NewGuid(), request.Name);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return category;
    }
}
