using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetAllCategories;

// TODO: paging, filtering
public record GetAllCategoriesQuery() : IRequest<ErrorOr<List<Category>>>;
