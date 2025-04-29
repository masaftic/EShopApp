using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Orders.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Queries;

public record GetOrdersByUserIdQuery(int UserId) : IRequest<ErrorOr<List<OrderDto>>>;

public class GetOrdersByUserIdHandler : IRequestHandler<GetOrdersByUserIdQuery, ErrorOr<List<OrderDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersByUserIdHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.UserId == request.UserId)
            .ProjectToType<OrderDto>()
            .ToListAsync(cancellationToken);

        return orders;
    }
}
