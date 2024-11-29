using EShopApp.Domain.Entities;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IRepository<T> where T : Entity<Guid>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}