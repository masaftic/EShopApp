using EShopApp.Domain.Entities;
using ErrorOr;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IUserService
{
    Task<ErrorOr<User>> GetUserByIdAsync(Guid userId);
    Task<ErrorOr<User>> GetUserByEmailAsync(string email);
    Task<ErrorOr<bool>> RegisterUserAsync(User user, string password);
    Task<bool> CheckPasswordAsync(Guid userId, string password);
}
