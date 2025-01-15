using EShopApp.Api.Models.Responses;
using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.Entities;

namespace EShopApp.Api.Mappers;

public static class ProductMappers
{
    public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse(product.Id, product.Name, product.Quantity, product.Price.ToString(),
            product.Description, product.CategoryId);
    }

    public static PaginatedList<ProductResponse> ToPaginatedListResponse(this PaginatedList<Product> products)
    {
        return new PaginatedList<ProductResponse>(
            products.Items.Select(p => p.ToProductResponse()).ToList(), products.TotalCount, products.PageSize, products.PageNumber);
    }
}