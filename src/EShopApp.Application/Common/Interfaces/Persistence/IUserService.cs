using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.Entities;
using FluentResults;

namespace EShopApp.Application.Common.Interfaces.Persistence;

public interface IUserService
{
    Task<Result<User>> GetUserByIdAsync(Guid userId);
    Task<Result<User>> GetUserByEmailAsync(string email);
    Task<Result> RegisterUserAsync(User user, string password);
}
