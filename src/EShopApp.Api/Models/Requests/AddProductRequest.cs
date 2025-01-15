namespace EShopApp.Api.Models.Requests;

public record AddProductRequest(
    string Name,
    int Quantity,
    decimal Price,
    string Description,
    int CategoryId
);