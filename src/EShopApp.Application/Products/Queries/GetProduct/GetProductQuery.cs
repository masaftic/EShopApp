using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetProduct;

public record GetProductQuery(
    int Id) : IRequest<ErrorOr<Product>>;