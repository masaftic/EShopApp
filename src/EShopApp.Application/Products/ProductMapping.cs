using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using Mapster;

namespace EShopApp.Application.Products;

public class ProductMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.Quantity, src => src.Inventory.Stock - src.Inventory.ReservedStock) // Code smell, TODO: refactor this
            .Map(dest => dest.CategoryName, src => src.Category.Name)
            .Map(dest => dest.Images, src => src.Images);
        
        config.NewConfig<ProductImage, ProductImageDto>()
            .Map(dest => dest.ImageUrl, src => src.ImageKey)
            .Map(dest => dest.OriginalFileName, src => src.OriginalFileName)
            .Map(dest => dest.IsMain, src => src.IsMain);
    }
}