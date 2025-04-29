using ErrorOr;
using EShopApp.Application.Carts.DTOs;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using EShopApp.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Carts.Commands.Checkout;

public record CheckoutCommand() : IRequest<ErrorOr<PaymentIntentResult>>;

public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, ErrorOr<PaymentIntentResult>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPaymentService _paymentService;
    private readonly IReservationService _reservationService;

    public CheckoutCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IPaymentService paymentService, IReservationService reservationService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _paymentService = paymentService;
        _reservationService = reservationService;
    }

    public async Task<ErrorOr<PaymentIntentResult>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var cart = await _dbContext.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .SingleAsync(c => c.UserId == userId, cancellationToken: cancellationToken);

        if (cart.CartItems.Count == 0)
            return Error.Validation(description: "Cannot checkout on an empty cart");

        if (cart.SessionExpiryDate is not null && cart.SessionExpiryDate > DateTime.UtcNow)
        {
            var result = await _reservationService.ExtendExistingReservationAsync(userId, cancellationToken)
                .ThenAsync(reservation => _paymentService.GetPaymentIntentAsync(reservation.PaymentIntentId));

            return result;
        }

        // Update cart item prices
        foreach (var cartItem in cart.CartItems)
        {
            cartItem.UpdatePrice(cartItem.Product!.Price);
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var paymentIntentResult = await CreatePaymentIntentAsync(cart, userId);
            if (paymentIntentResult.IsError) return paymentIntentResult.Errors;

            var reservationResult = await _reservationService.CreateReservationAsync(userId, paymentIntentResult.Value.PaymentIntentId, cart.CartItems.ToList(), cancellationToken);
            if (reservationResult.IsError) return reservationResult.Errors;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return paymentIntentResult.Value;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Error.Failure("An error occurred while processing the checkout.", ex.Message);
        }
    }

    private async Task<ErrorOr<PaymentIntentResult>> CreatePaymentIntentAsync(Cart cart, int userId)
    {
        var options = new PaymentIntentOptionsDto
        {
            Amount = (long)(cart.TotalPrice * 100), // in smallest currency unit (e.g., cents for USD)
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                { "cart_id", cart.Id.ToString() },
                { "user_id", userId.ToString() }
            }
        };

        return await _paymentService.CreatePaymentIntentAsync(options);
    }
}