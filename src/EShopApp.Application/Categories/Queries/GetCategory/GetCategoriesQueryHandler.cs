using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, ErrorOr<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryQueryHandler(ICategoryRepository CategoryRepository)
    {
        _categoryRepository = CategoryRepository;
    }

    public async Task<ErrorOr<Category>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var Category = await _categoryRepository.GetByIdAsync(request.Id);
        if (Category is null)
            return Errors.Category.NotFound;

        return Category;
    }
}