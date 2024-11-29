namespace EShopApp.Api.Models.Requests;

public record AddProductRequest(
    string Name,
    int Quantity,
    decimal PriceAmount,
    string PriceCurrency,
    string Description,
    Guid CategoryId
);