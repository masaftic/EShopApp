using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string SortBy,
    string SortOrder,
    int PageNumber,
    int PageSize
) : IRequest<ErrorOr<PaginatedList<ProductDto>>>;