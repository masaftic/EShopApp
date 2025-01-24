using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Orders.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Queries;

public record GetOrderByIdQuery(int Id) : IRequest<ErrorOr<OrderDto>>;


public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, ErrorOr<OrderDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetOrderByIdHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ErrorOr<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .Where(o => o.Id == request.Id)
            .ProjectToType<OrderDto>()
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (order is null)
            return Error.NotFound(description: "Order not found");

        return order;
    }
}