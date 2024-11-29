using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ErrorOr<List<Category>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository CategoryRepository)
    {
        _categoryRepository = CategoryRepository;
    }


    public async Task<ErrorOr<List<Category>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return (await _categoryRepository.GetAllAsync()).ToList();
    }
}