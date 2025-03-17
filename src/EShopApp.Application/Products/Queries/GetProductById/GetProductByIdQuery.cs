using ErrorOr;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(
    int Id) : IRequest<ErrorOr<ProductDto>>;