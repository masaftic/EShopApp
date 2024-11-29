using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetProduct;

public record GetProductQuery(
    Guid Id) : IRequest<ErrorOr<Product>>;