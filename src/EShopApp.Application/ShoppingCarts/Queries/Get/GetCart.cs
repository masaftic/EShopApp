using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.ShoppingCarts.Commands.AddToCart;
using EShopApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.ShoppingCarts.Queries.Get;

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
            .Include(c => c.CartItems)
            .SingleOrDefaultAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (userCart == null)
            return Error.NotFound("Cart.NotFound");

        return userCart.ToDto();
    }
}