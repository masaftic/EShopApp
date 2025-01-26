using EShopApp.Application.Inventories.Commands.AddInventory;
using EShopApp.Domain.Entities;
using Mapster;

namespace EShopApp.Application.Inventories;

public class InventoryMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Inventory, InventoryDto>();
    }
}