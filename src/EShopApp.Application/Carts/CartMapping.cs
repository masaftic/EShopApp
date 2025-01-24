using EShopApp.Application.Carts.DTOs;
using EShopApp.Domain.Entities;
using Mapster;

namespace EShopApp.Application.Carts;

public class CartMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Cart, CartDto>();
        
        config.NewConfig<CartItem, CartItemDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.ProductDescription, src => src.Product.Description);
    }
}