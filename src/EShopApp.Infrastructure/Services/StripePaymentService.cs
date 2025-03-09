using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.Commands.CreatePayment;
using EShopApp.Application.Payments.DTOs;
using Stripe;

namespace EShopApp.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{

    public async Task<ErrorOr<PaymentIntentResult>> CreatePaymentIntentAsync(PaymentIntentOptionsDto options)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = options.Amount,
                Currency = options.Currency,
                Metadata = options.Metadata,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions()
                {
                    Enabled = true,
                }
            };

            var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);

            return new PaymentIntentResult(paymentIntent.Id, paymentIntent.Status, paymentIntent.ClientSecret);
        }
        catch (StripeException e)
        {
            Console.WriteLine(e);
            return Error.Conflict(description: e.StripeError.Message);
        }
    }
}
