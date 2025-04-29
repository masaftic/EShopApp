using ErrorOr;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Payments.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace EShopApp.Infrastructure.Payment;

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
                },
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
                paymentIntent.Metadata,
                null // Shipping address not typically available immediately
            );
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
            var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId, new PaymentIntentGetOptions
            {
                Expand = new List<string> { "shipping" } // Ensure shipping details are expanded
            });

            Domain.ValueObjects.Address? shippingAddress = null;
            if (paymentIntent.Shipping?.Address != null)
            {
                shippingAddress = new Domain.ValueObjects.Address(
                    paymentIntent.Shipping.Address.Line1 ?? string.Empty,
                    paymentIntent.Shipping.Address.Line2 ?? string.Empty,
                    paymentIntent.Shipping.Address.City ?? string.Empty,
                    paymentIntent.Shipping.Address.State ?? string.Empty,
                    paymentIntent.Shipping.Address.Country ?? string.Empty,
                    paymentIntent.Shipping.Address.PostalCode ?? string.Empty
                );
            }

            return new PaymentIntentResult(
                paymentIntent.Id,
                paymentIntent.Status,
                paymentIntent.ClientSecret,
                paymentIntent.Amount,
                paymentIntent.AmountReceived,
                paymentIntent.Currency,
                paymentIntent.Description,
                paymentIntent.Metadata,
                shippingAddress
            );
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe API error getting PaymentIntent: {PaymentIntentId}", paymentIntentId);
            return Error.Failure(description: $"Failed to retrieve payment intent: {e.StripeError?.Message ?? e.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting PaymentIntent: {PaymentIntentId}", paymentIntentId);
            return Error.Unexpected(description: "An unexpected error occurred while retrieving payment details.");
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
