namespace EShopApp.Api.Models.Requests;

public record UpdateProductRequest(
    string Name,
    decimal Price,
    string Description,
    int CategoryId
);