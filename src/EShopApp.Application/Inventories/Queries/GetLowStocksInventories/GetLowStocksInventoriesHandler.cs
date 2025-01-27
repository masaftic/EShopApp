using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Inventories.Commands.AddInventory;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Queries.GetLowStocksInventories;

public record GetLowStocksInventoriesQuery : IRequest<ErrorOr<List<InventoryDto>>>;


public class GetLowStocksInventoriesHandler : IRequestHandler<GetLowStocksInventoriesQuery, ErrorOr<List<InventoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetLowStocksInventoriesHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<InventoryDto>>> Handle(GetLowStocksInventoriesQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Inventories
            .Where(i => i.Stock <= i.ReorderLevel)
            .ProjectToType<InventoryDto>()
            .ToListAsync(cancellationToken);
    }
}