using EShopApp.Domain.Entities;
using MediatR;
using ErrorOr;

namespace EShopApp.Application.Categories.Commands.Add;

public record AddCategoryCommand(
    string Name,
    int? ParentId) : IRequest<ErrorOr<Category>>;