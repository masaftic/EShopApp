using ErrorOr;
using EShopApp.Application.Payments.Commands.CreatePayment;
using EShopApp.Application.Payments.DTOs;

namespace EShopApp.Application.Common.Interfaces.Services;

public interface IPaymentService
{
    public Task<ErrorOr<PaymentIntentResult>> CreatePaymentIntentAsync(PaymentIntentOptionsDto options);
}