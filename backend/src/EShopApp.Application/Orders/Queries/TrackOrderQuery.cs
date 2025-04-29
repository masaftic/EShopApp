using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Orders.DTOs;
using EShopApp.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Queries;

public record TrackOrderQuery(int OrderId) : IRequest<ErrorOr<OrderTrackingDto>>;

public class TrackOrderQueryHandler : IRequestHandler<TrackOrderQuery, ErrorOr<OrderTrackingDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public TrackOrderQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<OrderTrackingDto>> Handle(TrackOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .AsNoTracking() 
            .Select(o => new { o.Id, o.OrderNumber, o.Status }) 
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return DomainErrors.Order.NotFound;
        }

        var trackingDto = new OrderTrackingDto(order.Id, order.OrderNumber, order.Status);

        return trackingDto;
    }
}
