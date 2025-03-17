using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Inventories.Commands.AddInventory;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Inventories.Queries.GetInventory;

public record GetInventoryByIdQuery(int Id) : IRequest<ErrorOr<InventoryDto>>;

public class GetInventoryByIdHandler : IRequestHandler<GetInventoryByIdQuery, ErrorOr<InventoryDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetInventoryByIdHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<InventoryDto>> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _dbContext.Inventories
            .Where(p => p.Id == request.Id)
            .ProjectToType<InventoryDto>()
            .FirstOrDefaultAsync(cancellationToken);
        
        if (inventory is null)
            return Errors.Inventory.NotFound(request.Id);

        return inventory;
    }
}