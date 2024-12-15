using System.Windows.Input;
using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.ShoppingCarts.Commands.AddToCart;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.ShoppingCarts.Commands.ClearCart;

public record ClearCartCommand : IRequest<ErrorOr<Success>>;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public ClearCartCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var userCart = await _dbContext.Carts
            .Include(c => c.CartItems)
            .SingleOrDefaultAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (userCart == null)
            return Error.NotFound("Cart.NotFound");

        _dbContext.CartItems.RemoveRange(userCart.CartItems);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}