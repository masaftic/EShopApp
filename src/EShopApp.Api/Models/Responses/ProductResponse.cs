namespace EShopApp.Api.Models.Responses;

public record ProductResponse(
    Guid Id,
    string Name,
    int Quantity,
    decimal PriceAmount,
    string PriceCurrency,
    string Description,
    Guid CategoryId
);
