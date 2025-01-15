using ErrorOr;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Commands.Add;

public record AddProductCommand(
    string Name,
    int Quantity,
    decimal Price,
    string Description,
    int CategoryId) : IRequest<ErrorOr<Product>>;