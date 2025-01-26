using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Inventories.Commands.AddInventory;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Queries.GetInventory;

public record GetAllInventoriesQuery : IRequest<ErrorOr<List<InventoryDto>>>;


public class GetAllInventoriesHandler : IRequestHandler<GetAllInventoriesQuery, ErrorOr<List<InventoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllInventoriesHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<InventoryDto>>> Handle(GetAllInventoriesQuery request, CancellationToken cancellationToken)
    {
        var inventories = await _dbContext.Inventories
            .ProjectToType<InventoryDto>()
            .ToListAsync(cancellationToken);

        return inventories;
    }
}