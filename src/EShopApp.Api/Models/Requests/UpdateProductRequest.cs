namespace EShopApp.Api.Models.Requests;

public record UpdateProductRequest(
    string Name,
    int Quantity,
    decimal PriceAmount,
    string PriceCurrency,
    string Description,
    int CategoryId
);