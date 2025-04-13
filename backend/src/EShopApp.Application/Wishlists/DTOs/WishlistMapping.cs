using EShopApp.Domain.Entities;
using Mapster;

namespace EShopApp.Application.Wishlists.DTOs;

public class WishlistMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Wishlist, WishlistDto>()
            .Map(dest => dest.Items, src => src.WishlistItems.ToList());
        
        config.NewConfig<WishlistItem, WishlistItemDto>()
            .Map(dest => dest.ProductName, src => src.Product.Name)
            .Map(dest => dest.Price, src => src.Product.Price);
    }
}