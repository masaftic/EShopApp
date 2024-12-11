using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(
    int Id) : IRequest<ErrorOr<Category>>;