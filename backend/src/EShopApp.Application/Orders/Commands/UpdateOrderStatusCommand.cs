using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Enums;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, OrderStatus NewStatus) : IRequest<ErrorOr<Success>>;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid order status provided.");
    }
}

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;


    public UpdateOrderStatusCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
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

        if (!IsValidStatusTransition(order.Status, request.NewStatus))
        {
            return DomainErrors.Order.InvalidStatusTransition;
        }

        order.Status = request.NewStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        if (currentStatus == OrderStatus.Shipped && newStatus == OrderStatus.Pending)
        {
            return false; 
        }

        if (currentStatus == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
        {
            return false; 
        }

        return true; 
    }
}
