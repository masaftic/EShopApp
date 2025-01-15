namespace EShopApp.Api.Models.Requests;

public record UpdateProductRequest(
    string Name,
    int Quantity,
    decimal Price,
    string Description,
    int CategoryId
);