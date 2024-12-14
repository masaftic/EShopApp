using ErrorOr;
using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Interfaces.Services;

public interface ICategoryService
{
    public Task<ErrorOr<List<Category>>> GetByPathIdsAsync(List<int> pathIds);
    public Task<ErrorOr<List<Category>>> GetByPathNamesAsync(List<string> names);
}