using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetAllProducts;

public record GetProductsQuery(
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    int PageNumber,
    int PageSize
) : IRequest<ErrorOr<PaginatedList<Product>>>;