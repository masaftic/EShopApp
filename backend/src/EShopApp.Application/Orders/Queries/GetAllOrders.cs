using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Orders.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Queries;

public record GetAllOrdersQuery() : IRequest<ErrorOr<List<OrderDto>>>;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, ErrorOr<List<OrderDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetAllOrdersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .ProjectToType<OrderDto>() 
            .ToListAsync(cancellationToken);

        return orders;
    }
}