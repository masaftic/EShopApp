using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategories;

// TODO: paging, filtering
public record GetCategoriesQuery(string[]? Segments) : IRequest<ErrorOr<List<Category>>>;
