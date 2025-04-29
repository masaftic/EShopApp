using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Orders.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<ErrorOr<OrderDto>>;


public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, ErrorOr<OrderDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderByIdHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Where(o => o.Id == request.Id)
            .ProjectToType<OrderDto>()
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (order is null)
            return DomainErrors.Order.NotFound;

        return order;
    }
}