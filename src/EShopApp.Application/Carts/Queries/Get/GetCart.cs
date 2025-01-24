using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Queries.Get;

public record GetCartQuery : IRequest<ErrorOr<CartDto>>;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, ErrorOr<CartDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetCartQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var userCart = await _dbContext.Carts
            .Where(c => c.UserId == userId)
            .ProjectToType<CartDto>()
            .SingleOrDefaultAsync(cancellationToken);

        if (userCart == null)
            return Error.NotFound("Cart.NotFound");

        return userCart;
    }
}