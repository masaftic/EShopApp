using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(
    int Id) : IRequest<ErrorOr<Category>>;