using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Orders.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.ValueObjects;
using Mapster;

namespace EShopApp.Application.Orders;

public class OrderMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.OrderItems, src => src.OrderItems.ToArray());

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.ProductName, src => src.Product!.Name)
            .Map(dest => dest.ProductDescription, src => src.Product!.Description);
        
        config.NewConfig<Address, AddressDto>();
    }
}
