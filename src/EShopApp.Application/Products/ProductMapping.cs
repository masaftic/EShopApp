using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using Mapster;

namespace EShopApp.Application.Products;

public class ProductMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.Quantity, src => src.Inventory.Stock - src.Inventory.ReservedStock)
            .Map(dest => dest.CategoryName, src => src.Category.Name);
    }
}