namespace EShopApp.Api.Models.Requests;

public record AddCartItemRequest(int ProductId, int Quantity);