using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Categories.Queries.GetCategoriesByPath;

public record GetCategoriesByPathQuery(
    string Path) : IRequest<ErrorOr<List<Category>>>;