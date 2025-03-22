
using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Orders.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetOrderByUser() : IRequest<ErrorOr<List<OrderDto>>>;


public class GetOrderByUserHandler : IRequestHandler<GetOrderByUser, ErrorOr<List<OrderDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderByUserHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<List<OrderDto>>> Handle(GetOrderByUser request, CancellationToken cancellationToken)
    {
        int userId = int.Parse(_currentUserService.UserId);

        var orders = await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .ProjectToType<OrderDto>()
            .ToListAsync(cancellationToken: cancellationToken);

        return orders;
    }
}