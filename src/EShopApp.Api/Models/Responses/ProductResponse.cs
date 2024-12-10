using EShopApp.Domain.ValueObjects;

namespace EShopApp.Api.Models.Responses;

public record ProductResponse(
    int Id,
    string Name,
    int Quantity,
    string Price,
    string Description,
    int CategoryId
);
