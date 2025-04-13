using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using EShopApp.Infrastructure.Payment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace EShopApp.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeApiCredentials _stripeApiCredentials;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(IOptions<StripeApiCredentials> stripeApiCredentials, ILogger<StripePaymentService> logger)
    {
        _stripeApiCredentials = stripeApiCredentials.Value;
        _logger = logger;
    }

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

            return new PaymentIntentResult(
                paymentIntent.Id,
                paymentIntent.Status,
                paymentIntent.ClientSecret,
                paymentIntent.Amount,
                paymentIntent.AmountReceived,
                paymentIntent.Currency,
                paymentIntent.Description,
                paymentIntent.Metadata);
        }
        catch (StripeException e)
        {
            Console.WriteLine(e);
            return Error.Conflict(description: e.StripeError.Message);
        }
    }

    public async Task<ErrorOr<PaymentIntentResult>> GetPaymentIntentAsync(string paymentIntentId)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

            return new PaymentIntentResult(
                paymentIntent.Id,
                paymentIntent.Status,
                paymentIntent.ClientSecret,
                paymentIntent.Amount,
                paymentIntent.AmountReceived,
                paymentIntent.Currency,
                paymentIntent.Description,
                paymentIntent.Metadata);
        }
        catch (StripeException e)
        {
            Console.WriteLine(e);
            return Error.Conflict(description: e.StripeError.Message);
        }
    }

    public ErrorOr<PaymentStatusResponse> ProcessWebhook(string rawJson, string Signature)
    {
        _logger.LogInformation("Processing Stripe webhook event.");

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(rawJson, Signature, _stripeApiCredentials.WebhookSecret);
            _logger.LogInformation("Stripe webhook event constructed successfully.");
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Failed to parse Stripe webhook event.");
            return Error.Conflict(description: "Failed to parse stripe webhook event");
        }

        if (stripeEvent.Data.Object is PaymentIntent intent)
        {
            _logger.LogInformation("Processing PaymentIntent with ID: {PaymentIntentId}", intent.Id);

            PaymentStatus status = stripeEvent.Type switch
            {
                EventTypes.PaymentIntentSucceeded => PaymentStatus.Succeeded,
                EventTypes.PaymentIntentPaymentFailed => PaymentStatus.Failed,
                EventTypes.PaymentIntentProcessing => PaymentStatus.Processing,
                _ => PaymentStatus.Unknown
            };

            string failureReason = intent.LastPaymentError?.Message ?? "Unknown error";

            _logger.LogInformation("PaymentIntent status: {Status}, Failure reason: {FailureReason}", status, failureReason);

            return new PaymentStatusResponse(intent.Id, status, failureReason);
        }

        _logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.Type);
        return new PaymentStatusResponse(null!, PaymentStatus.Unknown, "Unknown error");
    }
}
