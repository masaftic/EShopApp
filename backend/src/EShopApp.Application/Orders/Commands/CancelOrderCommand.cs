using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Commands;

public record CancelOrderCommand(int OrderId) : IRequest<ErrorOr<Success>>;

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return DomainErrors.Order.NotFound;
        }

        int userId = int.Parse(_currentUserService.UserId);

        if (order.UserId != userId && !_currentUserService.IsInRole("Admin"))
        {
            return DomainErrors.Order.Unauthorized;
        }

        var cancelResult = order.Cancel();

        if (cancelResult.IsError)
        {
            return cancelResult.Errors;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
