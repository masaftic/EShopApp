using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetAllProducts;

// TODO: paging, filtering
public record GetAllProductsQuery() : IRequest<ErrorOr<List<Product>>>;
